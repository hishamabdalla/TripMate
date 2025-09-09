using Tripmate.Application.Services.Identity.ForgotPassword;
using Tripmate.Application.Services.Identity.ForgotPassword.DTO;
using Tripmate.Application.Services.Identity.Login;
using Tripmate.Application.Services.Identity.Login.DTOs;
using Tripmate.Application.Services.Identity.RefreshTokens;
using Tripmate.Application.Services.Identity.RefreshTokens.DTOs;
using Tripmate.Application.Services.Identity.Register;
using Tripmate.Application.Services.Identity.Register.DTOs;
using Tripmate.Application.Services.Identity.ResetPassword;
using Tripmate.Application.Services.Identity.ResetPassword.DTO;
using Tripmate.Application.Services.Identity.VerifyEmail.DTOs;
using Tripmate.Domain.Common.Response;
using Tripmate.Domain.Services.Interfaces.Identity;

namespace Tripmate.Application.Services.Identity
{
    public class AuthService(
        ILoginHandler loginHandler,
        IRegisterHandler registerHandler,
        IResetPasswordHandler resetPassword,
        IForgetPasswordHandler forgetPassword,
        IRefreshTokenHandler refreshTokenHandler)
        : IAuthService
    {
        public async Task<ApiResponse<TokenResponse>> LoginAsync(LoginDto loginDto)
        {
            return await loginHandler.HandleLoginAsync(loginDto);
        }
        public async Task<ApiResponse<string>> RegisterAsync(RegisterDto registerDto)
        {
            return await registerHandler.HandleRegisterAsync(registerDto);
        }
        public async Task<ApiResponse<string>> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto)
        {
            return await forgetPassword.ForgetPassword(forgotPasswordDto);
        }
        public async Task<ApiResponse<string>> ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
        {
            return await resetPassword.ResetPassword(resetPasswordDto);
        }
        public async Task<ApiResponse<string>> VerifyEmail(VerifyEmailDto verifyEmailDto)
        {
            return await registerHandler.VerifyEmail(verifyEmailDto);
        }

        public async Task<ApiResponse<TokenResponse>> RefreshTokenAsync(RefreshTokenDto refreshToken)
        {
            return await refreshTokenHandler.HandleRefreshTokenAsync(refreshToken);

        }
    }
}
