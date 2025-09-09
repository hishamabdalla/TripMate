using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Cryptography;
using Tripmate.Application.Services.Abstractions.Identity;
using Tripmate.Application.Services.Identity.Register.DTOs;
using Tripmate.Application.Services.Identity.VerifyEmail;
using Tripmate.Application.Services.Identity.VerifyEmail.DTOs;
using Tripmate.Domain.Common.Response;
using Tripmate.Domain.Entities.Models;

namespace Tripmate.Application.Services.Identity.Register
{
    public class RegisterHandler(
        UserManager<ApplicationUser> userManager,
        IMemoryCache cache,
        ITokenService tokenService,
        IEmailHandler emailHandler)
        : IRegisterHandler
    {
        private const string PendingUsersCacheKey = "PendingUsers_";

        public async Task<ApiResponse<string>> HandleRegisterAsync(RegisterDto registerDto)
        {
            var existingUser = await userManager.FindByEmailAsync(registerDto.Email);
            if (existingUser != null)
            {
                return new ApiResponse<string>(false, 400, "User already exists .",
                    errors: new List<string>() { "Email is already registered" });
            }

            var existingUserName = await userManager.FindByNameAsync(registerDto.UserName);
            if (existingUserName != null)
            {
                return new ApiResponse<string>(false, 400, "Username already exists",
                    errors: new List<string>() { "Username is already taken" });
            }

            if (cache.TryGetValue($"{PendingUsersCacheKey}{registerDto.Email}", out PendingUserData existingPending))
            {
                var timeSinceLastRequest = DateTime.UtcNow-(existingPending.Expiration.AddHours(-24));
                if (timeSinceLastRequest.TotalMinutes<5)
                {
                    return new ApiResponse<string>(false, 400,
                    errors: new List<string>() { "Verification email already send,please wait 5 minutes before requesting another." });
                }
                cache.Remove($"{PendingUsersCacheKey}{registerDto.Email}");
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
                await emailHandler.SendVerificationEmail(pendingUser.Email, pendingUser.VerificationCode);
                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(pendingUser.Expiration)
                    .SetPriority(CacheItemPriority.Normal);

                cache.Set($"{PendingUsersCacheKey}{pendingUser.Email}", pendingUser, cacheOptions);
                return new ApiResponse<string>(true, 200, "Verification email sent",data: pendingUser.Email);
            }
            catch (Exception ex)
            {
                return new ApiResponse<string>(false, 400, "Failed to send verification email");
               
            }
        }

        public async Task<ApiResponse<string>> VerifyEmail(VerifyEmailDto verifyEmailDto)
        {
            if (!cache.TryGetValue($"{PendingUsersCacheKey}{verifyEmailDto.Email}", out PendingUserData pendingUser))
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
            };
            var result = await userManager.CreateAsync(user, pendingUser.Password);

            if (!result.Succeeded)
            {
                return new ApiResponse<string>(false, 400,
                    errors: new List<string>() { "User creation failed" });
            }
            cache.Remove($"{PendingUsersCacheKey}{verifyEmailDto.Email}");
            
            var token = await tokenService.GenerateTokenAsync(user);
            return new ApiResponse<string>(true, 200, "Registration completed successfully");

        }

        private string GenerateVerificationCode()
        {
            var randomNumber = new byte[4];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToHexString(randomNumber)[..6].ToUpper();
        }
        
    }
}
