using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Tripmate.Application.Services.Abstractions.Identity;
using Tripmate.Application.Services.Identity.RefreshTokens.DTOs;
using Tripmate.Domain.Common.Response;
using Tripmate.Domain.Entities.Models;

namespace Tripmate.Application.Services.Identity.RefreshTokens
{
    public class RefreshTokenHandler : IRefreshTokenHandler
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITokenService _tokenService;
        private readonly ILogger<RefreshTokenHandler> _logger;

        public RefreshTokenHandler(
            UserManager<ApplicationUser> userManager,
            ITokenService tokenService,
            ILogger<RefreshTokenHandler> logger)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _logger = logger;
        }

        public async Task<ApiResponse<TokenResponse>> HandleRefreshTokenAsync(RefreshTokenDto refreshTokenDto)
        {
            _logger.LogInformation("Processing refresh token request");

            if (string.IsNullOrEmpty(refreshTokenDto.RefreshToken))
            {
                _logger.LogWarning("Refresh token request failed: Empty or null refresh token provided");
                return new ApiResponse<TokenResponse>(false, 400, "Invalid refresh token");
            }

            // Logic to validate the refresh token and generate a new access token



            var user =await _userManager.Users.SingleOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == refreshTokenDto.RefreshToken));

            if (user ==null)
            {
                _logger.LogWarning("Refresh token request failed: No user found with the provided refresh token");
                return new ApiResponse<TokenResponse>(false, 400, "Invalid refresh token",
                    errors: new List<string>() { "The provided refresh token is invalid." });
            }

            // Check if the refresh token is still valid
            var refreshTokenEntity = user.RefreshTokens.Single(t => t.Token == refreshTokenDto.RefreshToken);


            if (!refreshTokenEntity.IsActive)
            {
                _logger.LogWarning("Refresh token request failed: The provided refresh token is inactive");
                return new ApiResponse<TokenResponse>(false, 400, "Inactive refresh token",
                    errors: new List<string>() { "The provided refresh token is inactive." });
            }

            _logger.LogInformation("Refresh token is valid, generating new tokens");

            // Revoke the old refresh token
            refreshTokenEntity.RevokedOn = DateTime.UtcNow;

            var newRefreshToken = _tokenService.GenerateRefreshToken();
            user.RefreshTokens.Add(newRefreshToken);
            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                _logger.LogError("Failed to update user refresh tokens: {Errors}",
                    string.Join(", ", updateResult.Errors.Select(e => e.Description)));
                return new ApiResponse<TokenResponse>(false, 500, "Failed to update refresh token");
            }

            _logger.LogInformation("User's refresh tokens updated successfully");


            // Generate a new access token

            _logger.LogInformation("Generating new access token");

            var tokenResponse = await _tokenService.GenerateTokenAsync(user);
            tokenResponse.RefreshToken = newRefreshToken.Token;
            tokenResponse.RefreshTokenExpiration = newRefreshToken.Expiration;

            _logger.LogInformation("New tokens generated successfully");

            return  new ApiResponse<TokenResponse>(tokenResponse)
            {
                Message = "Refresh token successfully processed.",
                StatusCode = 200,
                Success = true
            
            };



        }
    }
}
