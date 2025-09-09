using Tripmate.Application.Services.Regions.DTOs;
using Tripmate.Domain.Common.Response;
using Tripmate.Domain.Specification.Regions;

namespace Tripmate.Application.Services.Abstractions.Region
{
    public interface IRegionService
    {
        Task<ApiResponse<IEnumerable<RegionDto>>> GetAllRegionForCountryAsync(int countryId);
        Task<ApiResponse<RegionDto>> GetRegionByIdAsync(int regionId);
        Task<ApiResponse<RegionDto>> CreateRegionAsync(SetRegionDto regionDto);
        Task<ApiResponse<RegionDto>> UpdateRegionAsync(int id, SetRegionDto regionDto);
        Task<ApiResponse<bool>> DeleteRegionAsync(int id);
        Task<PaginationResponse<IEnumerable<RegionDto>>> GetRegionsAsync(RegionParameters parameters);


    }
}
