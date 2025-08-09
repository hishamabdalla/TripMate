using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
using Tripmate.Application.Services.Identity.VerifyEmail;
using Tripmate.Application.Services.Identity.VerifyEmail.DTOs;
using Tripmate.Domain.Common.Response;
using Tripmate.Domain.Services.Interfaces.Identity;

namespace Tripmate.Application.Services.Identity
{
    public class AuthService : IAuthService
    {
        private readonly ILoginHandler _loginHandler;
        private readonly IRegisterHandler _registerHandler;
        private readonly IRefreshTokenHandler _refreshTokenHandler;
        public AuthService(ILoginHandler loginHandler, IRegisterHandler registerHandler,IRefreshTokenHandler refreshTokenHandler)
        private readonly IResetPasswordHandler _resetPassword;
        private readonly IForgetPasswordHandler _forgetPassword;

        public AuthService(ILoginHandler loginHandler, IRegisterHandler registerHandler, IResetPasswordHandler resetPassword, IForgetPasswordHandler forgetPassword)
        {
            _registerHandler = registerHandler;
            _resetPassword=resetPassword;
            _forgetPassword=forgetPassword;
            _loginHandler = loginHandler;
            _refreshTokenHandler = refreshTokenHandler;


        }

        

        public async Task<ApiResponse<TokenResponse>> LoginAsync(LoginDto loginDto)
        {
            return await _loginHandler.HandleLoginAsync(loginDto);
        }

      

        public async Task<ApiResponse<string>> RegisterAsync(RegisterDto registerDto)
        {
            return await _registerHandler.HandleRegisterAsync(registerDto);
        }
        public async Task<ApiResponse<string>> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto)
        {
            return await _forgetPassword.ForgetPassword(forgotPasswordDto);
        }
        public async Task<ApiResponse<string>> ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
        {
            return await _resetPassword.ResetPassword(resetPasswordDto);
        }
        public async Task<ApiResponse<string>> VerifyEmail(VerifyEmailDto verifyEmailDto)
        {
            return await _registerHandler.VerifyEmail(verifyEmailDto);
        }

        public async Task<ApiResponse<TokenResponse>> RefreshTokenAsync(RefreshTokenDto refreshToken)
        {
            return await _refreshTokenHandler.HandleRefreshTokenAsync(refreshToken);

        }
    }
}
