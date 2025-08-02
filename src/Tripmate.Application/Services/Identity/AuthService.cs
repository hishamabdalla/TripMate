using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tripmate.Application.Services.Identity.Login;
using Tripmate.Application.Services.Identity.Login.DTOs;
using Tripmate.Application.Services.Identity.Register;
using Tripmate.Application.Services.Identity.Register.DTOs;
using Tripmate.Domain.Common.Response;
using Tripmate.Domain.Services.Interfaces.Identity;

namespace Tripmate.Application.Services.Identity
{
    public class AuthService : IAuthService
    {
        private readonly ILoginHandler _loginHandler;
        private readonly IRegisterHandler _registerHandler;

        public AuthService(ILoginHandler loginHandler, IRegisterHandler registerHandler)
        {

            _registerHandler = registerHandler;
            _loginHandler = loginHandler;

        }

        public async Task<ApiResponse<TokenResponse>> LoginAsync(LoginDto loginDto)
        {
            return await _loginHandler.HandleLoginAsync(loginDto);
        }

        public async Task<ApiResponse<string>> RegisterAsync(RegisterDto registerDto)
        {
            return await _registerHandler.HandleRegisterAsync(registerDto);
        }
    }
}
