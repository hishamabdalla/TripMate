using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tripmate.Application.Reviews.DTOs;
using Tripmate.Application.Services.Reviews;
using Tripmate.Domain.Common.Response;
using Tripmate.Domain.Enums;

namespace Tripmate.Application.Reviews
{
    public interface IReviewService
    {
        Task<PaginationResponse<IEnumerable<ReadReviewDto>>> GetReviewsByEntityAsync(int entityId, ReviewType reviewType, int pageNumber = 1, int pageSize = 10);
        Task<ApiResponse<ReadReviewDto>> GetReviewByIdAsync(int id);
        Task<PaginationResponse<IEnumerable<ReadReviewDto>>> GetReviewsByUserIdAsync(int pageNumber = 1, int pageSize = 10);
        Task<ApiResponse<double>> GetAverageRatingAsync(int entityId, ReviewType reviewType);
        Task<ApiResponse<int>> GetReviewCountAsync(int entityId, ReviewType reviewType);
        Task<ApiResponse<ReadReviewDto>> AddReviewAsync(AddReviewDto addReviewDto);
        Task<ApiResponse<ReadReviewDto>> UpdateReviewAsync(int id, UpdateReviewDto updateReviewDto);
        Task<ApiResponse<bool>> DeleteReviewAsync(int id);
    }
}

