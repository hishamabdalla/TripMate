using Tripmate.Application.Services.Identity.Login.DTOs;
using Tripmate.Domain.Common.Response;

namespace Tripmate.Application.Services.Identity.Login
{
    public interface ILoginHandler
    {
        Task<ApiResponse<TokenResponse>> HandleLoginAsync(LoginDto loginDto);
    }
}
