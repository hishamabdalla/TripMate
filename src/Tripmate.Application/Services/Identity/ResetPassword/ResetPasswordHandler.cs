using Microsoft.AspNetCore.Identity;
using Tripmate.Application.Services.Identity.ResetPassword.DTO;
using Tripmate.Domain.Common.Response;
using Tripmate.Domain.Entities.Models;

namespace Tripmate.Application.Services.Identity.ResetPassword
{
    public class ResetPasswordHandler(UserManager<ApplicationUser> userManager) : IResetPasswordHandler
    {
        public async Task<ApiResponse<string>> ResetPassword(ResetPasswordDto resetPasswordDto)
        {
            var user = await userManager.FindByEmailAsync(resetPasswordDto.Email);
            if (user==null)
                return new ApiResponse<string>(false, 400, "Invalid request.");

            var result = await userManager.ResetPasswordAsync(user, resetPasswordDto.Code,resetPasswordDto.NewPassword);
            if (!result.Succeeded)
                return new ApiResponse<string>(false, 404, "Password reset failed.");

            return new ApiResponse<string>(true, 200, "Password has been reset successfully!");
        }
    }
}
