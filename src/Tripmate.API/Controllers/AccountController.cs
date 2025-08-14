using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tripmate.Application.Services.Identity.ForgotPassword.DTO;
using Tripmate.Application.Services.Identity.Login.DTOs;
using Tripmate.Application.Services.Identity.RefreshTokens.DTOs;
using Tripmate.Application.Services.Identity.Register.DTOs;
using Tripmate.Application.Services.Identity.ResetPassword.DTO;
using Tripmate.Application.Services.Identity.VerifyEmail.DTOs;
using Tripmate.Domain.Common.Response;
using Tripmate.Domain.Services.Interfaces.Identity;

namespace Tripmate.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AccountController(IAuthService authService)
        {
            _authService = authService;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            var response = await _authService.RegisterAsync(registerDto);
            if (!response.Success)
            {
                return BadRequest(response);

            }
            return Ok(response);
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var response = await _authService.LoginAsync(loginDto);
            if (!response.Success)
            {
                return BadRequest(response);
            }
            
            if(!string.IsNullOrEmpty(response.Data.RefreshToken))
            {
                SetRefreshTokenInCookie(response.Data.RefreshToken, response.Data.RefreshTokenExpiration);

            }


            return Ok(response);
        }
        [HttpPost("VerifyEmail")]
        public async Task<IActionResult> VerifyEmail(VerifyEmailDto verifyEmailDto)
        {
            var result = await _authService.VerifyEmail(verifyEmailDto);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

       

        [HttpGet("refresh-token")]
        public async Task<IActionResult> RefreshToken()
        {

            var refreshToken = Request.Cookies["refreshToken"];
            
            if (string.IsNullOrEmpty(refreshToken))
            {
                return BadRequest(new ApiResponse<TokenResponse>(false, 400, "Refresh token is missing"));
            }


            var response = await _authService.RefreshTokenAsync(refreshToken);
            if (!response.Success)
            {
                return BadRequest(response);
            }
            // Set the new refresh token in the cookie
            if (!string.IsNullOrEmpty(response.Data.RefreshToken))
            {
                SetRefreshTokenInCookie(response.Data.RefreshToken, response.Data.RefreshTokenExpiration);
            }
            return Ok(response);
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

        [HttpPost("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordDto forgotPasswordDto)
        {
            var result = await _authService.ForgotPasswordAsync(forgotPasswordDto);
            if (result.Errors!=null&&result.Errors.Any())
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto resetPasswordDto)
        {
            var result = await _authService.ResetPasswordAsync(resetPasswordDto);
            if (result.Errors!=null&&result.Errors.Any())
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
    }
}
