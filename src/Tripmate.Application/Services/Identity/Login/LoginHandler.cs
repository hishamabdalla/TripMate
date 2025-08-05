using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tripmate.Application.Services.Abstractions.Identity;
using Tripmate.Application.Services.Identity.Login.DTOs;
using Tripmate.Domain.Common.Response;
using Tripmate.Domain.Entities;

namespace Tripmate.Application.Services.Identity.Login
{
    public class LoginHandler:ILoginHandler
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITokenService tokenService;
        public LoginHandler(UserManager<ApplicationUser> userManager, ITokenService tokenService)
        {
            _userManager = userManager;
            this.tokenService = tokenService;
        }

        public async Task<ApiResponse<TokenResponse>> HandleLoginAsync(LoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null)
            {
                return new ApiResponse<TokenResponse>(false, 404, "User not found",
                    errors: new List<string>() { "Invalid email or password" });
            }
            var isPasswordValid = await _userManager.CheckPasswordAsync(user, loginDto.Password);
            if (!isPasswordValid)
            {
                return new ApiResponse<TokenResponse>(false, 400, "Invalid credentials",
                    errors: new List<string>() { "Invalid email or password" });
            }
            var tokenResponse = await tokenService.GenerateTokenAsync(user);
            return new ApiResponse<TokenResponse>(tokenResponse);
        }
    }
}
