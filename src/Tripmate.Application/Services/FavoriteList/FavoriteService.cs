using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tripmate.Application.Services.Abstractions.Favorite;
using Tripmate.Application.Services.FavoriteList.DTOS;
using Tripmate.Domain.Common.Response;
using Tripmate.Domain.Specification.FavoriteList;

namespace Tripmate.Application.Services.FavoriteList
{
    public class FavoriteService : IFavoriteService
    {

        public Task<PaginationResponse<IEnumerable<FavoriteResponseDto>>> GetUserFavoritesAsync(string userId, FavoriteParameters parameters)
        {
            throw new NotImplementedException();
        }
        public Task<ApiResponse<FavoriteResponseDto>> AddFavoriteAsync(string userId, AddFavoriteDto addFavoriteDto)
        {
            throw new NotImplementedException();
        }
        public Task<ApiResponse<bool> DeleteFavoriteAsync(int favoriteId, string userId)
        {
            throw new NotImplementedException();
        }
    }
}
