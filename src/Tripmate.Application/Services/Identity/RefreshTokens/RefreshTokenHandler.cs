using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tripmate.Application.Services.Abstractions.Identity;
using Tripmate.Application.Services.Identity.RefreshTokens.DTOs;
using Tripmate.Domain.Common.Response;
using Tripmate.Domain.Entities.Models;

namespace Tripmate.Application.Services.Identity.RefreshTokens
{
    public class RefreshTokenHandler(UserManager<ApplicationUser> userManager, ITokenService tokenService)
        : IRefreshTokenHandler
    {
        public async Task<ApiResponse<TokenResponse>> HandleRefreshTokenAsync(RefreshTokenDto refreshTokenDto)
        {

            // Logic to validate the refresh token and generate a new access token

            var user =await userManager.Users.SingleOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == refreshTokenDto.RefreshToken));

            if (user ==null)
            {
                return new ApiResponse<TokenResponse>(false, 400, "Invalid refresh token",
                    errors: new List<string>() { "The provided refresh token is invalid." });
            }

            // Check if the refresh token is still valid
            var refreshTokenEntity = user.RefreshTokens.Single(t => t.Token == refreshTokenDto.RefreshToken);
            if (!refreshTokenEntity.IsActive)
            {
                return new ApiResponse<TokenResponse>(false, 400, "Inactive refresh token",
                    errors: new List<string>() { "The provided refresh token is inactive." });
            }

            //revoke the old refresh token
            refreshTokenEntity.RevokedOn = DateTime.UtcNow;

            var newRefreshToken = tokenService.GenerateRefreshToken();
            user.RefreshTokens.Add(newRefreshToken);
            await userManager.UpdateAsync(user);

            // Generate a new access token
            var tokenResponse = await tokenService.GenerateTokenAsync(user);
            tokenResponse.RefreshToken = newRefreshToken.Token;
            tokenResponse.RefreshTokenExpiration = newRefreshToken.Expiration;

            return  new ApiResponse<TokenResponse>(tokenResponse)
            {
                Message = "Refresh token successfully processed.",
                StatusCode = 200,
                Success = true
            };
        }
    }
}
