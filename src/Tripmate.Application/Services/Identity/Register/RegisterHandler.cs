using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tripmate.Application.Services.Identity.Register.DTOs;
using Tripmate.Domain.Common.Response;
using Tripmate.Domain.Entities;

namespace Tripmate.Application.Services.Identity.Register
{
    public class RegisterHandler : IRegisterHandler
    {
        private readonly UserManager<ApplicationUser> _userManager;
        public RegisterHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<ApiResponse<string>> HandleRegisterAsync(RegisterDto registerDto)
        {
            var existingUser = await _userManager.FindByEmailAsync(registerDto.Email);

            if (existingUser != null)
            {
                return new ApiResponse<string>(false, 400, "User already exists",
                    errors: new List<string>() { "Email is already registered" });
            }

            var existingUserName = await _userManager.FindByNameAsync(registerDto.UserName);
            if (existingUserName != null)
            {
                return new ApiResponse<string>(false, 400, "Username already exists",
                    errors: new List<string>() { "Username is already taken" });
            }

            var newUser = new ApplicationUser
            {
                UserName = registerDto.UserName,
                Email = registerDto.Email,
                PhoneNumber = registerDto.PhoneNumber,
                Country = registerDto.Country
            };

            var result = await _userManager.CreateAsync(newUser, registerDto.Password);
            if (!result.Succeeded)
            {
                return new ApiResponse<string>(false, 400, "User registration failed",
                    errors: result.Errors.Select(e => e.Description).ToList());
            }

            return new ApiResponse<string>(true, 200, "User registered successfully");
             
        }
    }
}
