using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
