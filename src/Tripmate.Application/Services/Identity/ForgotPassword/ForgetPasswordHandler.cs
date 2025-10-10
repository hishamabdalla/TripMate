using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Tripmate.Application.Services.Identity.ForgotPassword.DTO;
using Tripmate.Application.Services.Identity.VerifyEmail;
using Tripmate.Domain.Common.Response;
using Tripmate.Domain.Entities.Models;

namespace Tripmate.Application.Services.Identity.ForgotPassword
{
    public class ForgetPasswordHandler:IForgetPasswordHandler
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IEmailHandler emailHandler;
        private readonly ILogger<ForgetPasswordHandler> logger;
        public ForgetPasswordHandler(UserManager<ApplicationUser> userManager, IEmailHandler emailHandler, ILogger<ForgetPasswordHandler> logger)
        {
            this.userManager = userManager;
            this.emailHandler = emailHandler;
            this.logger = logger;
        }


        public async Task<ApiResponse<string>> ForgetPassword(ForgotPasswordDto forgotPasswordDto)
        {
            var user = await userManager.FindByEmailAsync(forgotPasswordDto.Email);
            if (user == null)
            {
                logger.LogWarning("User with email {Email} does not exist.", forgotPasswordDto.Email);
                return new ApiResponse<string>(false, 400,"User with this email does not exist.");

            }
            var token = await userManager.GeneratePasswordResetTokenAsync(user);
            logger.LogInformation("Generated password reset token for user {Email}.", forgotPasswordDto.Email);

            await emailHandler.SendResetCodeEmail(forgotPasswordDto.Email, token);
            logger.LogInformation("Sent password reset email to {Email}.", forgotPasswordDto.Email);
            return new ApiResponse<string>(true, 200, "If the email is correct, a password reset token has been sent.");


        }
    }
}
