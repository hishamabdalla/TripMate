using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tripmate.Application.Services.Identity.ForgotPassword.DTO;
using Tripmate.Application.Services.Identity.ResetPassword.DTO;
using Tripmate.Domain.Common.Response;

namespace Tripmate.Application.Services.Identity.ResetPassword
{
    public interface IResetPasswordHandler
    {
        Task<ApiResponse<string>> ResetPassword(ResetPasswordDto resetPasswordDto);
    }
}
