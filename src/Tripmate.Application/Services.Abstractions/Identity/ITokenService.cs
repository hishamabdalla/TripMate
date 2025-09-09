using Tripmate.Domain.Common.Response;
using Tripmate.Domain.Entities.Models;

namespace Tripmate.Application.Services.Abstractions.Identity
{
    public interface ITokenService
    {
        Task<TokenResponse> GenerateTokenAsync(ApplicationUser user);
        RefreshToken GenerateRefreshToken();

    }
}
