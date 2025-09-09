using Microsoft.AspNetCore.Identity;
using Tripmate.Application.Services.Identity.ForgotPassword.DTO;
using Tripmate.Application.Services.Identity.VerifyEmail;
using Tripmate.Domain.Common.Response;
using Tripmate.Domain.Entities.Models;

namespace Tripmate.Application.Services.Identity.ForgotPassword
{
    public class ForgetPasswordHandler(UserManager<ApplicationUser> userManager, IEmailHandler emailHandler)
        : IForgetPasswordHandler
    {
        public async Task<ApiResponse<string>> ForgetPassword(ForgotPasswordDto forgotPasswordDto)
        {
            var user = await userManager.FindByEmailAsync(forgotPasswordDto.Email);
            if (user == null)
                return new ApiResponse<string>(false, 400, "If the account exists, you will receive a password reset token.");
            var token = await userManager.GeneratePasswordResetTokenAsync(user);
            await emailHandler.SendResetCodeEmail(forgotPasswordDto.Email, token);
            return new ApiResponse<string>(true, 200, "If the email is correct, a password reset token has been sent.");
        }
    }
}
