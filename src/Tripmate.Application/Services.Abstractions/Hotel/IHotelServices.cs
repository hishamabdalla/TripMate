using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tripmate.Application.Services.Hotels.DTOS;
using Tripmate.Domain.Common.Response;
using Tripmate.Domain.Specification.Hotels;
using Tripmate.Domain.Specification.Restaurants;

namespace Tripmate.Application.Services.Abstractions.Hotel
{
    public interface IHotelServices
    {
        Task<PaginationResponse<IEnumerable<ReadHotelDto>>> GetHotelsAsync(HotelsParameters parameters);
        Task<ApiResponse<ReadHotelDto>> GetHotelByIdAsync(int id);
        Task<ApiResponse<IEnumerable<ReadHotelDto>>> GetHotelsByRegionIdAsync(int regionId);
        Task<ApiResponse<ReadHotelDto>> AddHotelAsync(AddHotelDto addHotelDto);
        Task<ApiResponse<ReadHotelDto>> UpdateHotelAsync(UpdateHotelDto updateHotelDto);
        Task<ApiResponse<bool>> DeleteHotel(int id);
    }
}
