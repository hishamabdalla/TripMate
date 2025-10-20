using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Tripmate.Application.Reviews.DTOs;
using Tripmate.Application.Services.Reviews;
using Tripmate.Domain.Common.Response;
using Tripmate.Domain.Common.User;
using Tripmate.Domain.Entities.Models;
using Tripmate.Domain.Enums;
using Tripmate.Domain.Exceptions;
using Tripmate.Domain.Interfaces;
using Tripmate.Domain.Specification.Reviews;

namespace Tripmate.Application.Reviews
{
    public class ReviewService : IReviewService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<ReviewService> _logger;
        private readonly IUserContext _userContext;

        public ReviewService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<ReviewService> logger, IUserContext userContext)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _userContext = userContext;
        }

        public async Task<ApiResponse<ReadReviewDto>> AddReviewAsync(AddReviewDto addReviewDto)
        {
            var user = _userContext.GetCurrentUser();
            var userId = user.Id;

            _logger.LogInformation("Adding new review for {ReviewType} ID: {EntityId} by user: {UserId}",
                           addReviewDto.ReviewType, addReviewDto.EntityId, userId);

            await ValidateEntityExistsAsync(addReviewDto.EntityId, addReviewDto.ReviewType);

            var existingReview = await GetUserReviewForEntityAsync(userId, addReviewDto.EntityId, addReviewDto.ReviewType);

            if(existingReview != null)
            {
                _logger.LogWarning("User {UserId} has already submitted a review for {ReviewType} ID: {EntityId}",
                                   userId, addReviewDto.ReviewType, addReviewDto.EntityId);
                throw new BadRequestException("You have already submitted a review for this entity.");
            }
            
            var review = _mapper.Map<Review>(addReviewDto);
            review.UserId = userId;

            await _unitOfWork.Repository<Review, int>().AddAsync(review);
            await _unitOfWork.SaveChangesAsync();

            var spec = new ReviewSpecification(review.Id, addReviewDto.ReviewType);
            var createdReview = await _unitOfWork.Repository<Review, int>().GetByIdWithSpecAsync(spec);
            var readReviewDto = _mapper.Map<ReadReviewDto>(createdReview);
            return new ApiResponse<ReadReviewDto>(readReviewDto)
            {
                Message = "Review added successfully."
            };



        }

        public async Task<ApiResponse<ReadReviewDto>> GetReviewByIdAsync(int id)
        {
            _logger.LogInformation("Getting review by ID: {ReviewId}", id);

            var review=await _unitOfWork.Repository<Review, int>().GetByIdAsync(id);
            if (review == null)
            {
                _logger.LogWarning("Review not found with ID: {ReviewId}", id);
                throw new NotFoundException($"Review with ID {id} not found");
            }
            var reviewType = DetermineReviewType(review);



            var spec = new ReviewSpecification(id,reviewType);
            var reviewWithDetails = await _unitOfWork.Repository<Review, int>().GetByIdWithSpecAsync(spec);

           
            var reviewDto = _mapper.Map<ReadReviewDto>(reviewWithDetails);
            _logger.LogInformation("Successfully retrieved review with ID: {ReviewId}", id);

            return new ApiResponse<ReadReviewDto>(reviewDto);
        }

        public async Task<PaginationResponse<IEnumerable<ReadReviewDto>>> GetReviewsByEntityAsync(
            int entityId, 
            ReviewType reviewType, 
            int pageNumber = 1, 
            int pageSize = 10)
        {
            _logger.LogInformation("Getting reviews for {ReviewType} with ID: {EntityId}", reviewType, entityId);

            var parameters = new ReviewParameters 
            { 
                EntityId = entityId, 
                ReviewType = reviewType,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
            
            var spec = new ReviewSpecification(parameters);
            var reviews = await _unitOfWork.Repository<Review, int>().GetAllWithSpecAsync(spec);
            var totalCount = await _unitOfWork.Repository<Review, int>().CountAsync(spec);

            var reviewDtos = _mapper.Map<IEnumerable<ReadReviewDto>>(reviews);
            
            _logger.LogInformation("Retrieved {Count} reviews for {ReviewType} ID: {EntityId}", 
                reviewDtos.Count(), reviewType, entityId);

            return new PaginationResponse<IEnumerable<ReadReviewDto>>(
                reviewDtos,
                pageNumber,
                pageSize,
                totalCount
            );
        }

        public async Task<PaginationResponse<IEnumerable<ReadReviewDto>>> GetReviewsByUserIdAsync(
            int pageNumber = 1, 
            int pageSize = 10)
        {
            var user = _userContext.GetCurrentUser();
            var userId = user.Id;

            _logger.LogInformation("Getting reviews for user ID: {UserId}", userId);

            var parameters = new ReviewParameters 
            { 
                UserId = userId, 
                PageNumber = pageNumber,
                PageSize = pageSize
            };
            
            var spec = new ReviewSpecification(parameters);
            var reviews = await _unitOfWork.Repository<Review, int>().GetAllWithSpecAsync(spec);
            var totalCount = await _unitOfWork.Repository<Review, int>().CountAsync(spec);

            var reviewDtos = _mapper.Map<IEnumerable<ReadReviewDto>>(reviews);
            
            _logger.LogInformation("Retrieved {Count} reviews for user ID: {UserId}", reviewDtos.Count(), userId);

            return new PaginationResponse<IEnumerable<ReadReviewDto>>(
                reviewDtos,
                pageNumber,
                pageSize,
                totalCount
            );
        }

        public async Task<ApiResponse<double>> GetAverageRatingAsync(int entityId, ReviewType reviewType)
        {
            _logger.LogInformation("Calculating average rating for {ReviewType} with ID: {EntityId}", reviewType, entityId);

            var parameters = new ReviewParameters 
            { 
                EntityId = entityId, 
                ReviewType = reviewType,
                PageSize = int.MaxValue 
            };
            
            var spec = new ReviewSpecification(parameters);
            var reviews = await _unitOfWork.Repository<Review, int>().GetAllWithSpecAsync(spec);

            if (!reviews.Any())
            {
                _logger.LogWarning("No reviews found for {ReviewType} ID: {EntityId}", reviewType, entityId);
                return new ApiResponse<double>(0)
                {
                    Message = "No reviews found"
                };
            }

            var averageRating = reviews.Average(r => r.Rating);
            _logger.LogInformation("Average rating for {ReviewType} ID {EntityId}: {Rating}", 
                reviewType, entityId, averageRating);

            return new ApiResponse<double>(Math.Round(averageRating, 2));
        }

        public async Task<ApiResponse<int>> GetReviewCountAsync(int entityId, ReviewType reviewType)
        {
            _logger.LogInformation("Getting review count for {ReviewType} with ID: {EntityId}", reviewType, entityId);

            var parameters = new ReviewParameters
            {
                EntityId = entityId,
                ReviewType = reviewType
            };

            var spec = new ReviewSpecification(parameters);
            var count = await _unitOfWork.Repository<Review, int>().CountAsync(spec);

            _logger.LogInformation("Review count for {ReviewType} ID {EntityId}: {Count}",
                reviewType, entityId, count);

            return new ApiResponse<int>(count);
        }

        public async Task<ApiResponse<ReadReviewDto>> UpdateReviewAsync(int id, UpdateReviewDto updateReviewDto)
        {
            var user = _userContext.GetCurrentUser();
            var userId = user.Id;

            _logger.LogInformation("Updating review with ID: {ReviewId} by user: {UserId}", id, userId);

            var existingReview = await _unitOfWork.Repository<Review, int>().GetByIdAsync(id);

            if (existingReview == null)
            {
                _logger.LogWarning("Review not found with ID: {ReviewId}", id);
                throw new NotFoundException($"Review with ID {id} not found");
            }

            if (existingReview.UserId != userId)
            {
                _logger.LogWarning("User {UserId} attempted to update review {ReviewId} owned by {OwnerId}", 
                    userId, id, existingReview.UserId);
                throw new BadRequestException("You can only update your own reviews");
            }

            existingReview.Rating = updateReviewDto.Rating;
            existingReview.Comment = updateReviewDto.Comment;

            _unitOfWork.Repository<Review, int>().Update(existingReview);
            await _unitOfWork.SaveChangesAsync();

            // Determine review type to load proper navigation properties
            var reviewType = existingReview.AttractionId.HasValue ? ReviewType.Attraction :
                            existingReview.RestaurantId.HasValue ? ReviewType.Restaurant : ReviewType.Hotel;

            var spec = new ReviewSpecification(id, reviewType);
            var updatedReview = await _unitOfWork.Repository<Review, int>().GetByIdWithSpecAsync(spec);
            var reviewDto = _mapper.Map<ReadReviewDto>(updatedReview);

            _logger.LogInformation("Successfully updated review with ID: {ReviewId}", id);
            return new ApiResponse<ReadReviewDto>(reviewDto)
            {
                Message = "Review updated successfully"
            };
        }

        public async Task<ApiResponse<bool>> DeleteReviewAsync(int id)
        {
            var user = _userContext.GetCurrentUser();
            var userId = user.Id;

            _logger.LogInformation("Deleting review ID: {ReviewId} by user: {UserId}", id, userId);

            var existingReview = await _unitOfWork.Repository<Review, int>().GetByIdAsync(id);

            if (existingReview == null)
            {
                _logger.LogWarning("Review not found with ID: {ReviewId}", id);
                throw new NotFoundException($"Review with ID {id} not found");
            }

            if (existingReview.UserId != userId)
            {
                _logger.LogWarning("User {UserId} attempted to delete review {ReviewId} owned by {OwnerId}", 
                    userId, id, existingReview.UserId);
                throw new BadRequestException("You can only delete your own reviews");
            }

            _unitOfWork.Repository<Review, int>().Delete(existingReview);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Successfully deleted review with ID: {ReviewId}", id);
            return new ApiResponse<bool>(true)
            {
                Message = "Review deleted successfully"
            };
        }

        private async Task ValidateEntityExistsAsync(int entityId, ReviewType reviewType)
        {
            switch (reviewType)
            {
                case ReviewType.Attraction:
                    var attraction = await _unitOfWork.Repository<Attraction, int>().GetByIdAsync(entityId);
                    if (attraction == null)
                    {
                        _logger.LogWarning("Attraction with ID: {EntityId} not found.", entityId);
                        throw new NotFoundException($"Attraction with ID {entityId} not found.");
                    }
                    break;

                case ReviewType.Restaurant:
                    var restaurant = await _unitOfWork.Repository<Restaurant, int>().GetByIdAsync(entityId);
                    if (restaurant == null)
                    {
                        _logger.LogWarning("Restaurant with ID: {EntityId} not found.", entityId);
                        throw new NotFoundException($"Restaurant with ID {entityId} not found.");
                    }
                    break;

                case ReviewType.Hotel:
                    var hotel = await _unitOfWork.Repository<Hotel, int>().GetByIdAsync(entityId);
                    if (hotel == null)
                    {
                        _logger.LogWarning("Hotel with ID: {EntityId} not found.", entityId);
                        throw new NotFoundException($"Hotel with ID {entityId} not found.");
                    }
                    break;

                default:
                    throw new BadRequestException("Invalid review type.");
            }
        }

        private async Task<Review> GetUserReviewForEntityAsync(string userId, int entityId, ReviewType reviewType)
        {
            var parameters = new ReviewParameters
            {
                UserId = userId,
                EntityId = entityId,
                ReviewType = reviewType,
                PageSize = 1
            };

            var spec = new ReviewSpecification(parameters);
            var reviews = await _unitOfWork.Repository<Review, int>().GetAllWithSpecAsync(spec);
            return reviews.FirstOrDefault() ?? null!;
        }

        private static ReviewType DetermineReviewType(Review review)
        { 
            if (review.AttractionId.HasValue)
                return ReviewType.Attraction;
            else if (review.RestaurantId.HasValue)
                return ReviewType.Restaurant;
            else
                return ReviewType.Hotel;

        }
        
    }
    
}
