using Tripmate.Application.Services.Identity.RefreshTokens.DTOs;
using Tripmate.Domain.Common.Response;

namespace Tripmate.Application.Services.Identity.RefreshTokens
{
    public interface IRefreshTokenHandler
    {
        Task<ApiResponse<TokenResponse>> HandleRefreshTokenAsync(RefreshTokenDto refreshTokenDto);
    }
}
