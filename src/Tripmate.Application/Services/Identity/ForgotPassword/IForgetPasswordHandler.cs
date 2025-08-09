using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tripmate.Application.Services.Identity.ForgotPassword.DTO;
using Tripmate.Application.Services.Identity.Register.DTOs;
using Tripmate.Domain.Common.Response;

namespace Tripmate.Application.Services.Identity.ForgotPassword
{
    public interface IForgetPasswordHandler
    {
        Task<ApiResponse<string>> ForgetPassword(ForgotPasswordDto forgotPasswordDto);
    }
}
