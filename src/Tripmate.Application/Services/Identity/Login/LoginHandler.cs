using Microsoft.AspNetCore.Identity;
using Tripmate.Application.Services.Abstractions.Identity;
using Tripmate.Application.Services.Identity.Login.DTOs;
using Tripmate.Domain.Common.Response;
using Tripmate.Domain.Entities.Models;

namespace Tripmate.Application.Services.Identity.Login
{
    public class LoginHandler(UserManager<ApplicationUser> userManager, ITokenService tokenService)
        : ILoginHandler
    {
        public async Task<ApiResponse<TokenResponse>> HandleLoginAsync(LoginDto loginDto)
        {
            var user = await userManager.FindByEmailAsync(loginDto.Email);
            var isPasswordValid = await userManager.CheckPasswordAsync(user, loginDto.Password);

            // Check if user is null or password is invalid

            if (user == null || !isPasswordValid)
            {
                return new ApiResponse<TokenResponse>(false, 400, "Invalid credentials",
                  errors: new List<string>() { "Invalid email or password" });
            }


            // Generate token

            var tokenResponse = await tokenService.GenerateTokenAsync(user);

            if(user.RefreshTokens.Any(t=> t.IsActive))
            {
                var activeRefreshToken = user.RefreshTokens.FirstOrDefault(t => t.IsActive);
                tokenResponse.RefreshToken = activeRefreshToken.Token;
                tokenResponse.RefreshTokenExpiration = activeRefreshToken.Expiration;
            }
            else
            {
                var newRefreshToken = tokenService.GenerateRefreshToken();
                user.RefreshTokens.Add(newRefreshToken);
                await userManager.UpdateAsync(user);

                tokenResponse.RefreshToken = newRefreshToken.Token;
                tokenResponse.RefreshTokenExpiration = newRefreshToken.Expiration;
            }

            return new ApiResponse<TokenResponse>(tokenResponse);
        }
    }
}
