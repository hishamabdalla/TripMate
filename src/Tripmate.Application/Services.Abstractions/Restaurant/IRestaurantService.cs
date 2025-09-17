using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tripmate.Application.Services.Attractions.DTOs;
using Tripmate.Application.Services.Restaurants.DTOS;
using Tripmate.Domain.Common.Response;
using Tripmate.Domain.Specification.Attractions;
using Tripmate.Domain.Specification.Restaurants;

namespace Tripmate.Application.Services.Abstractions.Restaurant
{
    public interface IRestaurantService
    {
        Task<PaginationResponse<IEnumerable<ReadRestaurantDto>>> GetRestaurantsAsync(RestaurantsParameters parameters);
        Task<ApiResponse<ReadRestaurantDto>> GetRestaurantByIdAsync(int id);
        Task<ApiResponse<IEnumerable<ReadRestaurantDto>>> GetRestaurantByRegionIdAsync(int regionId);
        Task<ApiResponse<ReadRestaurantDto>> AddRestaurantAsync(AddRestaurantDto addRestaurantDto);
        Task<ApiResponse<ReadRestaurantDto>> UpdateRestaurantAsync(UpdateRestaurantDto updateRestaurantDto);
        Task<ApiResponse<bool>> DeleteRestaurant(int id);
    }
}
