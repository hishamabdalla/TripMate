using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Tripmate.Application.Services.Abstractions.Identity;
using Tripmate.Application.Services.Identity.Register.DTOs;
using Tripmate.Application.Services.Identity.VerifyEmail;
using Tripmate.Application.Services.Identity.VerifyEmail.DTOs;
using Tripmate.Domain.Common.Response;
using Tripmate.Domain.Entities;

namespace Tripmate.Application.Services.Identity.Register
{
    public class RegisterHandler : IRegisterHandler
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMemoryCache _cache;
        private readonly ITokenService _tokenService;
        private readonly IEmailHandler _emailHandler;
        private const string PendingUsersCacheKey = "PendingUsers_";
        public RegisterHandler(UserManager<ApplicationUser> userManager, IMemoryCache cache, ITokenService tokenService, IEmailHandler emailHandler)
        {
            _userManager = userManager;
            _cache=cache;
            _tokenService=tokenService;
            _emailHandler=emailHandler;
        }
       
        public async Task<ApiResponse<string>> HandleRegisterAsync(RegisterDto registerDto)
        {
            var existingUser = await _userManager.FindByEmailAsync(registerDto.Email);
            if (existingUser != null)
            {
                return new ApiResponse<string>(false, 400, "User already exists",
                    errors: new List<string>() { "Email is already registered" });
            }

            var existingUserName = await _userManager.FindByNameAsync(registerDto.UserName);
            if (existingUserName != null)
            {
                return new ApiResponse<string>(false, 400, "Username already exists",
                    errors: new List<string>() { "Username is already taken" });
            }

            if (_cache.TryGetValue($"{PendingUsersCacheKey}{registerDto.Email}", out PendingUserData existingPending))
            {
                var timeSinceLastRequest = DateTime.UtcNow-(existingPending.Expiration.AddHours(-24));
                if (timeSinceLastRequest.TotalMinutes<5)
                {
                    return new ApiResponse<string>(false, 400,
                    errors: new List<string>() { "Verification email already send,please wait 5 minutes before requesting another." });
                }
                _cache.Remove($"{PendingUsersCacheKey}{registerDto.Email}");
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
                await _emailHandler.SendVerificationEmail(pendingUser.Email, pendingUser.VerificationCode);
                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(pendingUser.Expiration)
                    .SetPriority(CacheItemPriority.Normal);

                _cache.Set($"{PendingUsersCacheKey}{pendingUser.Email}", pendingUser, cacheOptions);
                return new ApiResponse<string>(true, 200, "Verification email sent");
            }
            catch (Exception ex)
            {
                return new ApiResponse<string>(false, 400, "Failed to send verification email");
               
            }
        }

        public async Task<ApiResponse<string>> VerifyEmail(VerifyEmailDto verifyEmailDto)
        {
            if (!_cache.TryGetValue($"{PendingUsersCacheKey}{verifyEmailDto.Email}", out PendingUserData pendingUser))
            {
                return new ApiResponse<string>(false, 400, "Invalid or expired verification request",
                    errors: new List<string>() { "Invalid or expired verification request" });
            }
            if (pendingUser.VerificationCode != verifyEmailDto.VerificationCode)
            {
                return new ApiResponse<string>(false, 400, "Invalid verification code");
            }
            var user = new ApplicationUser
            {
                Email = pendingUser.Email,
                UserName = pendingUser.UserName,
                PhoneNumber = pendingUser.PhoneNumber,
                VerificationCode=pendingUser.VerificationCode,
                Country=pendingUser.Country,
                EmailConfirmed = true,
                IsActive=true,
                IsEmailVerified = true,
            };
            var result = await _userManager.CreateAsync(user, pendingUser.Password);

            if (!result.Succeeded)
            {
                return new ApiResponse<string>(false, 400,
                    errors: new List<string>() { "User creation failed" });
            }
            _cache.Remove($"{PendingUsersCacheKey}{verifyEmailDto.Email}");
            
            var token = await _tokenService.GenerateTokenAsync(user);
            return new ApiResponse<string>(true, 200, "Registration completed successfully");

        }

        private string GenerateVerificationCode()
        {
            var randomNumber = new byte[4];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToHexString(randomNumber)[..6].ToUpper(); // 6-character code
        }
    }
}
