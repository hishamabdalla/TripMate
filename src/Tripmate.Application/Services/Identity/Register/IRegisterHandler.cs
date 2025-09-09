using Tripmate.Application.Services.Identity.Register.DTOs;
using Tripmate.Application.Services.Identity.VerifyEmail.DTOs;
using Tripmate.Domain.Common.Response;

namespace Tripmate.Application.Services.Identity.Register
{
    public interface IRegisterHandler
    {
        Task<ApiResponse<string>> HandleRegisterAsync(RegisterDto registerDto);
        Task<ApiResponse<string>> VerifyEmail(VerifyEmailDto verifyEmailDto);
    }
}
