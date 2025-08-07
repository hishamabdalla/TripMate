using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tripmate.Application.Services.Identity.ResetPassword.DTO;
using Tripmate.Domain.Common.Response;
using Tripmate.Domain.Entities.Models;

namespace Tripmate.Application.Services.Identity.ResetPassword
{
    public class ResetPasswordHandler : IResetPasswordHandler
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public ResetPasswordHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
        public async Task<ApiResponse<string>> ResetPassword(ResetPasswordDto resetPasswordDto)
        {
            var user = await _userManager.FindByEmailAsync(resetPasswordDto.Email);
            if (user==null)
                return new ApiResponse<string>(false, 400, "Invalid request.");

            var result = await _userManager.ResetPasswordAsync(user, resetPasswordDto.Code,resetPasswordDto.NewPassword);
            if (!result.Succeeded)
                return new ApiResponse<string>(false, 404, "Password reset failed.");

            return new ApiResponse<string>(true, 200, "Password has been reset successfully!");
        }
    }
}
