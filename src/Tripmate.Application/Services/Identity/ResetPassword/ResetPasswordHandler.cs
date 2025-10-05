using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Serilog;
using Tripmate.Application.Services.Identity.ResetPassword.DTO;
using Tripmate.Domain.Common.Response;
using Tripmate.Domain.Entities.Models;

namespace Tripmate.Application.Services.Identity.ResetPassword
{
    public class ResetPasswordHandler : IResetPasswordHandler

    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ILogger<ResetPasswordHandler> logger;
        public ResetPasswordHandler(UserManager<ApplicationUser> userManager, ILogger<ResetPasswordHandler> logger)
        {
            this.userManager = userManager;
            this.logger = logger;
        }
        public async Task<ApiResponse<string>> ResetPassword(ResetPasswordDto resetPasswordDto)
        {
         
            logger.LogInformation("Password reset attempt for user: {Email}", resetPasswordDto.Email);
            var user = await userManager.FindByEmailAsync(resetPasswordDto.Email);
            if (user == null)
            {
                logger.LogWarning("User with email {Email} does not exist.", resetPasswordDto.Email);
                return new ApiResponse<string>(false, 400, "Invalid request.");
            }

            var result = await userManager.ResetPasswordAsync(user, resetPasswordDto.Code,resetPasswordDto.NewPassword);
            logger.LogInformation("Reset password process completed for user: {Email}", resetPasswordDto.Email);

            if (!result.Succeeded)
            {
                logger.LogWarning("Password reset failed for user: {Email}. Errors: {Errors}", resetPasswordDto.Email, string.Join(", ", result.Errors.Select(e => e.Description)));
                return new ApiResponse<string>(false, 404, "Password reset failed.");

            }

            logger.LogInformation("Password has been reset successfully for user: {Email}", resetPasswordDto.Email);

            return new ApiResponse<string>(true, 200, "Password has been reset successfully!");
        }
    }
}
