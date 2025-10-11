using Tripmate.Application.Services.Attractions.DTOs;
using Tripmate.Domain.Common.Response;
using Tripmate.Domain.Specification.Attractions;

namespace Tripmate.Application.Services.Abstractions.Attraction
{
    public interface IAttractionService
    {
        Task<PaginationResponse<IEnumerable<AttractionDto>>> GetAttractionsAsync(AttractionParameter parameters);
        Task<ApiResponse<AttractionDto>> GetAttractionByIdAsync(int id);
        Task<ApiResponse<AttractionDto>> AddAsync(SetAttractionDto setAttractionDto);
        Task<ApiResponse<AttractionDto>> UpdateAsync(int id, SetAttractionDto attractionDto);
        Task<ApiResponse<bool>> DeleteAsync(int id);
       
    }
}
