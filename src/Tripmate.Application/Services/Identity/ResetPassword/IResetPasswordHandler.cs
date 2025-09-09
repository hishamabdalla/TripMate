using Tripmate.Application.Services.Identity.ResetPassword.DTO;
using Tripmate.Domain.Common.Response;

namespace Tripmate.Application.Services.Identity.ResetPassword
{
    public interface IResetPasswordHandler
    {
        Task<ApiResponse<string>> ResetPassword(ResetPasswordDto resetPasswordDto);
    }
}
