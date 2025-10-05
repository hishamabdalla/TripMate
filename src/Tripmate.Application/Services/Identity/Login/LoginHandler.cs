using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Tripmate.Application.Services.Abstractions.Identity;
using Tripmate.Application.Services.Identity.Login.DTOs;
using Tripmate.Domain.Common.Response;
using Tripmate.Domain.Entities.Models;

namespace Tripmate.Application.Services.Identity.Login
{
    public class LoginHandler : ILoginHandler
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITokenService _tokenService;
        private readonly ILogger<LoginHandler> _logger;

        public LoginHandler(UserManager<ApplicationUser> userManager, ITokenService tokenService, ILogger<LoginHandler> logger)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _logger = logger;
        }

        public async Task<ApiResponse<TokenResponse>> HandleLoginAsync(LoginDto loginDto)
        {
            _logger.LogInformation("Processing login request for email: {Email}", loginDto.Email);
            
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null)
            {
                _logger.LogWarning("Login failed: User not found for email: {Email}", loginDto.Email);
                return new ApiResponse<TokenResponse>(false, 400, "Invalid credentials",
                    errors: new List<string>() { "Invalid email or password" });
            }

            var isPasswordValid = await _userManager.CheckPasswordAsync(user, loginDto.Password);
            if (!isPasswordValid)
            {
                _logger.LogWarning("Login failed: Invalid password for user: {Email}", loginDto.Email);
                return new ApiResponse<TokenResponse>(false, 400, "Invalid credentials",
                    errors: new List<string>() { "Invalid email or password" });
            }

            _logger.LogInformation("User credentials validated successfully for: {Email}", loginDto.Email);

            // Generate token
            var tokenResponse = await _tokenService.GenerateTokenAsync(user);

            if (user.RefreshTokens.Any(t => t.IsActive))
            {
                var activeRefreshToken = user.RefreshTokens.FirstOrDefault(t => t.IsActive);
                tokenResponse.RefreshToken = activeRefreshToken.Token;
                tokenResponse.RefreshTokenExpiration = activeRefreshToken.Expiration;
                
                _logger.LogInformation("Using existing active refresh token for user: {Email}", loginDto.Email);
            }
            else
            {
                var newRefreshToken = _tokenService.GenerateRefreshToken();
                user.RefreshTokens.Add(newRefreshToken);
                await _userManager.UpdateAsync(user);

                tokenResponse.RefreshToken = newRefreshToken.Token;
                tokenResponse.RefreshTokenExpiration = newRefreshToken.Expiration;
                
                _logger.LogInformation("Generated new refresh token for user: {Email}", loginDto.Email);
            }

            _logger.LogInformation("Login successful for user: {Email}", loginDto.Email);
            return new ApiResponse<TokenResponse>(tokenResponse);
        }
    }
}
