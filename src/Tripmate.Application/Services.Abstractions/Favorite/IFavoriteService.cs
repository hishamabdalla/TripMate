using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tripmate.Application.Services.Countries.DTOs;
using Tripmate.Application.Services.FavoriteList.DTOS;
using Tripmate.Domain.Common.Response;
using Tripmate.Domain.Entities.Models;
using Tripmate.Domain.Specification.Countries;
using Tripmate.Domain.Specification.FavoriteList;

namespace Tripmate.Application.Services.Abstractions.Favorite
{
    public interface IFavoriteService
    {
        Task<PaginationResponse<IEnumerable<FavoriteResponseDto>>> GetUserFavoritesAsync(string userId, FavoriteParameters parameters);
        Task<ApiResponse<FavoriteResponseDto>> AddFavoriteAsync(string userId, AddFavoriteDto addFavoriteDto);
        Task<ApiResponse<bool>> DeleteFavoriteAsync(int favoriteId, string userId);
    }
}
