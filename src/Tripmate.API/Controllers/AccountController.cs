using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Tripmate.Application.Services.Identity.ForgotPassword.DTO;
using Tripmate.Application.Services.Identity.Login.DTOs;
using Tripmate.Application.Services.Identity.RefreshTokens.DTOs;
using Tripmate.Application.Services.Identity.Register.DTOs;
using Tripmate.Application.Services.Identity.ResetPassword.DTO;
using Tripmate.Application.Services.Identity.VerifyEmail.DTOs;
using Tripmate.Domain.Services.Interfaces.Identity;

namespace Tripmate.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous] // All account endpoints are public - no authentication required
    public class AccountController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AccountController> _logger;

        public AccountController(IAuthService authService, ILogger<AccountController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            _logger.LogInformation("Registration attempt for user: {Email}", registerDto.Email);
            
            var result = await _authService.RegisterAsync(registerDto);
            
            if (result.Success)
            {
                _logger.LogInformation("Successful registration for user: {Email}", registerDto.Email);
                return Ok(result);


            }
            else
            {
                _logger.LogWarning("Failed registration attempt for user: {Email}. Reason: {Message}", 
                    registerDto.Email, result.Message);

                return BadRequest(result);
            }
            
            
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            _logger.LogInformation("Login attempt for user: {Email}", loginDto.Email);
            
            var result = await _authService.LoginAsync(loginDto);
            
            if (!result.Success)
            {
                _logger.LogWarning("Failed login attempt for user: {Email}. Reason: {Message}", 
                    loginDto.Email, result.Message);
                return BadRequest(result);
            }
           
 
             if (!string.IsNullOrEmpty(result.Data?.RefreshToken))
             {
                SetRefreshTokenInCookie(result.Data.RefreshToken, result.Data.RefreshTokenExpiration);
             }

             _logger.LogInformation("Successful login for user: {Email}", loginDto.Email);
                return Ok(result);

        }

        [HttpPost("verify-email")]
        public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailDto verifyEmailDto)
        {
            _logger.LogInformation("Email verification attempt for user: {Email}", verifyEmailDto.Email);
            
            var result = await _authService.VerifyEmail(verifyEmailDto);
            
            if (!result.Success)
            {
                _logger.LogWarning("Failed email verification attempt for user: {Email}. Reason: {Message}", 
                    verifyEmailDto.Email, result.Message);
                return BadRequest(result);
            }
          

            _logger.LogInformation("Successful email verification for user: {Email}", verifyEmailDto.Email);
            return Ok(result);
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto forgotPasswordDto)
        {
            _logger.LogInformation("Forgot password request for user: {Email}", forgotPasswordDto.Email);
            
            var result = await _authService.ForgotPasswordAsync(forgotPasswordDto);

            if (!result.Success)
            {
                _logger.LogWarning("Failed forgot password request for user: {Email}. Reason: {Message}",
                    forgotPasswordDto.Email, result.Message);
                return BadRequest(result);
            }

            _logger.LogInformation("Processed forgot password request for user: {Email}", forgotPasswordDto.Email);
            return Ok(result);
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto)
        {
            _logger.LogInformation("Password reset attempt for user: {Email}", resetPasswordDto.Email);
            
            var result = await _authService.ResetPasswordAsync(resetPasswordDto);
            
            if (!result.Success)
            {

                _logger.LogWarning("Failed password reset attempt for user: {Email}. Reason: {Message}", 
                    resetPasswordDto.Email, result.Message);
                return BadRequest(result);

            }

            _logger.LogInformation("Successful password reset for user: {Email}",resetPasswordDto.Email);
            return Ok(result);



        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDto refreshTokenDto)
        {
            _logger.LogInformation("Refresh token request initiated");
            
            var result = await _authService.RefreshTokenAsync(refreshTokenDto);
            
            if (!result.Success)
            {
                _logger.LogWarning("Failed refresh token request. Reason: {Message}", result.Message);
                return BadRequest(result);

            }
            
            if (!string.IsNullOrEmpty(result.Data?.RefreshToken))
            {

                SetRefreshTokenInCookie(result.Data.RefreshToken, result.Data.RefreshTokenExpiration);
                _logger.LogInformation("Refresh token set in cookie successfully");

            }

            _logger.LogInformation("Refresh token request processed successfully");

            return Ok(result);
        }

        private void SetRefreshTokenInCookie(string refreshToken, DateTime expires)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true, // Set to true if using HTTPS
                Expires = expires.ToLocalTime()
            };

            Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
        }
    }
}
