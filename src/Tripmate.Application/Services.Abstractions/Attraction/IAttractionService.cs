using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tripmate.Application.Services.Attractions.DTOs;
using Tripmate.Domain.Common.Response;

namespace Tripmate.Application.Services.Abstractions.Attraction
{
    public interface IAttractionService
    {
        Task<ApiResponse<IEnumerable<AttractionDto>>> GetAllAttractionsAsync();
        Task<ApiResponse<AttractionDto>> GetAttractionByIdAsync(int id);
        Task<ApiResponse<AttractionDto>> AddAsync(SetAttractionDto setAttractionDto);
        Task<ApiResponse<AttractionDto>> UpdateAsync(int id, SetAttractionDto attractionDto);
        Task<ApiResponse<bool>> Delete(int id);
        Task<ApiResponse<IEnumerable<AttractionDto>>> GetAttractionsByRegionIdAsync(int regionId);
        Task<ApiResponse<IEnumerable<AttractionDto>>> GetAttractionsByTypeAsync(string type);
    }
}
