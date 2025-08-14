using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tripmate.Application.Services.Identity.ForgotPassword.DTO;
using Tripmate.Application.Services.Identity.Login.DTOs;
using Tripmate.Application.Services.Identity.RefreshTokens.DTOs;
using Tripmate.Application.Services.Identity.Register.DTOs;
using Tripmate.Application.Services.Identity.ResetPassword.DTO;
using Tripmate.Application.Services.Identity.VerifyEmail.DTOs;
using Tripmate.Domain.Common.Response;

namespace Tripmate.Domain.Services.Interfaces.Identity
{
    public interface IAuthService
    {
       Task<ApiResponse<TokenResponse>> LoginAsync(LoginDto loginDto);
       Task<ApiResponse<string>> RegisterAsync(RegisterDto registerDto);
       Task<ApiResponse<string>> VerifyEmail(VerifyEmailDto verifyEmailDto);
       Task<ApiResponse<TokenResponse>> RefreshTokenAsync(string refreshToken);
       Task<ApiResponse<string>> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto);
       Task<ApiResponse<string>> ResetPasswordAsync(ResetPasswordDto resetPasswordDto);
    }
}
