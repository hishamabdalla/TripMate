using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tripmate.Application.Services.Identity.RefreshTokens.DTOs;
using Tripmate.Domain.Common.Response;

namespace Tripmate.Application.Services.Identity.RefreshTokens
{
    public interface IRefreshTokenHandler
    {
        Task<ApiResponse<TokenResponse>> HandleRefreshTokenAsync(RefreshTokenDto refreshTokenDto);
    }
}
