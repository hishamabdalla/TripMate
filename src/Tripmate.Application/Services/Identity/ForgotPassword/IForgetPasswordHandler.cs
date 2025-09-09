using Tripmate.Application.Services.Identity.ForgotPassword.DTO;
using Tripmate.Domain.Common.Response;

namespace Tripmate.Application.Services.Identity.ForgotPassword
{
    public interface IForgetPasswordHandler
    {
        Task<ApiResponse<string>> ForgetPassword(ForgotPasswordDto forgotPasswordDto);
    }
}
