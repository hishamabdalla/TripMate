using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tripmate.Application.Services.Identity.ForgotPassword.DTO;
using Tripmate.Application.Services.Identity.VerifyEmail;
using Tripmate.Domain.Common.Response;
using Tripmate.Domain.Entities.Models;

namespace Tripmate.Application.Services.Identity.ForgotPassword
{
    public class ForgetPasswordHandler : IForgetPasswordHandler
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailHandler _emailHandler;
        public ForgetPasswordHandler(UserManager<ApplicationUser> userManager,IEmailHandler emailHandler)
        {
            _userManager=userManager;
            _emailHandler=emailHandler;
        }
        public async Task<ApiResponse<string>> ForgetPassword(ForgotPasswordDto forgotPasswordDto)
        {
            var user = await _userManager.FindByEmailAsync(forgotPasswordDto.Email);
            if (user == null)
                return new ApiResponse<string>(false, 400, "If the account exists, you will receive a password reset token.");
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            await _emailHandler.SendResetCodeEmail(forgotPasswordDto.Email, token);
            return new ApiResponse<string>(true, 200, "If the email is correct, a password reset token has been sent.");
        }
    }
}
