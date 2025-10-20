using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using Tripmate.Application.Services.Abstractions.Identity;
using Tripmate.Application.Services.Identity.Register.DTOs;
using Tripmate.Application.Services.Identity.VerifyEmail;
using Tripmate.Application.Services.Identity.VerifyEmail.DTOs;
using Tripmate.Domain.Common.Response;
using Tripmate.Domain.Entities.Models;

namespace Tripmate.Application.Services.Identity.Register
{
    public class RegisterHandler : IRegisterHandler
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMemoryCache _cache;
        private readonly ITokenService _tokenService;
        private readonly IEmailHandler _emailHandler;
        private readonly ILogger<RegisterHandler> _logger;
        
        private const string PendingUsersCacheKey = "PendingUsers_";

        public RegisterHandler(
            UserManager<ApplicationUser> userManager,
            IMemoryCache cache,
            ITokenService tokenService,
            IEmailHandler emailHandler,
            ILogger<RegisterHandler> logger)
        {
            _userManager = userManager;
            _cache = cache;
            _tokenService = tokenService;
            _emailHandler = emailHandler;
            _logger = logger;
        }

        public async Task<ApiResponse<string>> HandleRegisterAsync(RegisterDto registerDto)
        {
            _logger.LogInformation("Processing registration request for email: {Email}, username: {Username}", 
                registerDto.Email, registerDto.UserName);

            var existingUser = await _userManager.FindByEmailAsync(registerDto.Email);
            if (existingUser != null)
            {
                _logger.LogWarning("Registration failed: Email {Email} already exists", registerDto.Email);
                return new ApiResponse<string>(false, 400, "User already exists.",
                    errors: new List<string>() { "Email is already registered" });
            }

            var existingUserName = await _userManager.FindByNameAsync(registerDto.UserName);
            if (existingUserName != null)
            {
                _logger.LogWarning("Registration failed: Username {Username} already exists", registerDto.UserName);
                return new ApiResponse<string>(false, 400, "Username already exists",
                    errors: new List<string>() { "Username is already taken" });
            }

            if (_cache.TryGetValue($"{PendingUsersCacheKey}{registerDto.Email}", out PendingUserData existingPending))
            {
                var timeSinceLastRequest = DateTime.UtcNow - (existingPending.Expiration.AddHours(-24));
                if (timeSinceLastRequest.TotalMinutes < 5)
                {
                    _logger.LogWarning("Registration rate limited for email: {Email}. Time since last request: {Minutes} minutes", 
                        registerDto.Email, timeSinceLastRequest.TotalMinutes.ToString("F1"));
                    return new ApiResponse<string>(false, 400, message: "Verification email already sent, please wait 5 minutes before requesting another.");

                }
                _cache.Remove($"{PendingUsersCacheKey}{registerDto.Email}");
                _logger.LogInformation("Removed expired pending registration for email: {Email}", registerDto.Email);
            }
            
            var pendingUser = new PendingUserData
            {
                UserName = registerDto.UserName,
                Email = registerDto.Email,
                PhoneNumber = registerDto.PhoneNumber,
                Country = registerDto.Country,
                Password = registerDto.Password,
                VerificationCode = GenerateVerificationCode(),
                Expiration = DateTime.UtcNow.AddHours(24),
            };

            try
            {
                _logger.LogInformation("Attempting to send verification email to: {Email}", pendingUser.Email);
                await _emailHandler.SendVerificationEmail(pendingUser.Email, pendingUser.VerificationCode);
                
                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(pendingUser.Expiration)
                    .SetPriority(CacheItemPriority.Normal);

                _cache.Set($"{PendingUsersCacheKey}{pendingUser.Email}", pendingUser, cacheOptions);

                _logger.LogInformation("Successfully cached pending user and sent verification email to: {Email}. Cache expires at: {Expiration}",
                    pendingUser.Email, pendingUser.Expiration.ToString("d"));
                    
                return new ApiResponse<string>(true, 200, $"Verification email sent {pendingUser.VerificationCode} ", data: pendingUser.Email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send verification email to: {Email}", pendingUser.Email);
                return new ApiResponse<string>(false, 400, "Failed to send verification email");
            }
        }

        public async Task<ApiResponse<string>> VerifyEmail(VerifyEmailDto verifyEmailDto)
        {
            _logger.LogInformation("Processing email verification for: {Email} with code: {Code}", 
                verifyEmailDto.Email, verifyEmailDto.VerificationCode);

            if (!_cache.TryGetValue($"{PendingUsersCacheKey}{verifyEmailDto.Email}", out PendingUserData pendingUser))
            {
                _logger.LogWarning("Email verification failed: No pending registration found for email: {Email}", verifyEmailDto.Email);
                return new ApiResponse<string>(false, 400, "Invalid or expired verification request",
                    errors: new List<string>() { "Invalid or expired verification request" });
            }
            
            if (pendingUser.VerificationCode != verifyEmailDto.VerificationCode)
            {
                _logger.LogWarning("Email verification failed: Invalid verification code for email: {Email}. Expected: {Expected}, Received: {Received}", 
                    verifyEmailDto.Email, pendingUser.VerificationCode, verifyEmailDto.VerificationCode);
                return new ApiResponse<string>(false, 400, "Invalid verification code");
            }
            
            var user = new ApplicationUser
            {
                Email = pendingUser.Email,
                UserName = pendingUser.UserName,
                PhoneNumber = pendingUser.PhoneNumber,
                VerificationCode = pendingUser.VerificationCode,
                Country = pendingUser.Country,
                EmailConfirmed = true,
                IsActive = true,
            };
            
            _logger.LogInformation("Creating new user account for: {Email}", pendingUser.Email);
            var result = await _userManager.CreateAsync(user, pendingUser.Password);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                _logger.LogError("User creation failed for email: {Email}. Errors: {Errors}", pendingUser.Email, errors);
                return new ApiResponse<string>(false, 400,
                    errors: new List<string>() { "User creation failed" });
            }
            
            _cache.Remove($"{PendingUsersCacheKey}{verifyEmailDto.Email}");
            _logger.LogInformation("Successfully created user account and removed pending data for: {Email}", verifyEmailDto.Email);
            
            var token = await _tokenService.GenerateTokenAsync(user);
            _logger.LogInformation("Generated access token for newly registered user: {Email}", verifyEmailDto.Email);
            
            return new ApiResponse<string>(true, 200, "Registration completed successfully");
        }

        private string GenerateVerificationCode()
        {
            var randomNumber = new byte[4];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            var code = Convert.ToHexString(randomNumber)[..6].ToUpper();
            _logger.LogDebug("Generated verification code: {Code}", code);
            return code;
        }
    }
}
