using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Tripmate.Application.Reviews;
using Tripmate.Application.Reviews.DTOs;
using Tripmate.Domain.Enums;

namespace Tripmate.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewService _reviewService;
        private readonly ILogger<ReviewsController> _logger;
        
        public ReviewsController(IReviewService reviewService, ILogger<ReviewsController> logger)
        {
            _reviewService = reviewService;
            _logger = logger;
        }

        #region Get Endpoints

        /// <summary>
        /// Get a specific review by ID
        /// </summary>
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetReviewById(int id)
        {
            _logger.LogInformation("Getting review by ID: {ReviewId}", id);
            var result = await _reviewService.GetReviewByIdAsync(id);
            return Ok(result);
        }

        /// <summary>
        /// Get reviews for a specific attraction with pagination
        /// </summary>
        [HttpGet("attraction/{attractionId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAttractionReviews(
            int attractionId,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            _logger.LogInformation("Getting reviews for attraction ID: {AttractionId}", attractionId);
            var result = await _reviewService.GetReviewsByEntityAsync(attractionId, ReviewType.Attraction, pageNumber, pageSize);
            return Ok(result);
        }

        /// <summary>
        /// Get reviews for a specific restaurant with pagination
        /// </summary>
        [HttpGet("restaurant/{restaurantId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetRestaurantReviews(
            int restaurantId,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            _logger.LogInformation("Getting reviews for restaurant ID: {RestaurantId}", restaurantId);
            var result = await _reviewService.GetReviewsByEntityAsync(restaurantId, ReviewType.Restaurant, pageNumber, pageSize);
            return Ok(result);
        }

        /// <summary>
        /// Get reviews for a specific hotel with pagination
        /// </summary>
        [HttpGet("hotel/{hotelId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetHotelReviews(
            int hotelId,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            _logger.LogInformation("Getting reviews for hotel ID: {HotelId}", hotelId);
            var result = await _reviewService.GetReviewsByEntityAsync(hotelId, ReviewType.Hotel, pageNumber, pageSize);
            return Ok(result);
        }

        /// <summary>
        /// Get average rating for an attraction
        /// </summary>
        [HttpGet("attraction/{attractionId}/average-rating")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAttractionAverageRating(int attractionId)
        {
            _logger.LogInformation("Getting average rating for attraction ID: {AttractionId}", attractionId);
            var result = await _reviewService.GetAverageRatingAsync(attractionId, ReviewType.Attraction);
            return Ok(result);
        }

        /// <summary>
        /// Get average rating for a restaurant
        /// </summary>
        [HttpGet("restaurant/{restaurantId}/average-rating")]
        [AllowAnonymous]
        public async Task<IActionResult> GetRestaurantAverageRating(int restaurantId)
        {
            _logger.LogInformation("Getting average rating for restaurant ID: {RestaurantId}", restaurantId);
            var result = await _reviewService.GetAverageRatingAsync(restaurantId, ReviewType.Restaurant);
            return Ok(result);
        }

        /// <summary>
        /// Get average rating for a hotel
        /// </summary>
        [HttpGet("hotel/{hotelId}/average-rating")]
        [AllowAnonymous]
        public async Task<IActionResult> GetHotelAverageRating(int hotelId)
        {
            _logger.LogInformation("Getting average rating for hotel ID: {HotelId}", hotelId);
            var result = await _reviewService.GetAverageRatingAsync(hotelId, ReviewType.Hotel);
            return Ok(result);
        }

        /// <summary>
        /// Get review count for an attraction
        /// </summary>
        [HttpGet("attraction/{attractionId}/count")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAttractionReviewCount(int attractionId)
        {
            _logger.LogInformation("Getting review count for attraction ID: {AttractionId}", attractionId);
            var result = await _reviewService.GetReviewCountAsync(attractionId, ReviewType.Attraction);
            return Ok(result);
        }

        /// <summary>
        /// Get review count for a restaurant
        /// </summary>
        [HttpGet("restaurant/{restaurantId}/count")]
        [AllowAnonymous]
        public async Task<IActionResult> GetRestaurantReviewCount(int restaurantId)
        {
            _logger.LogInformation("Getting review count for restaurant ID: {RestaurantId}", restaurantId);
            var result = await _reviewService.GetReviewCountAsync(restaurantId, ReviewType.Restaurant);
            return Ok(result);
        }

        /// <summary>
        /// Get review count for a hotel
        /// </summary>
        [HttpGet("hotel/{hotelId}/count")]
        [AllowAnonymous]
        public async Task<IActionResult> GetHotelReviewCount(int hotelId)
        {
            _logger.LogInformation("Getting review count for hotel ID: {HotelId}", hotelId);
            var result = await _reviewService.GetReviewCountAsync(hotelId, ReviewType.Hotel);
            return Ok(result);
        }

        /// <summary>
        /// Get current user's reviews with pagination
        /// </summary>
        [HttpGet("my-reviews")]
        [Authorize]
        public async Task<IActionResult> GetMyReviews(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            _logger.LogInformation("Getting current user's reviews");
            var result = await _reviewService.GetReviewsByUserIdAsync(pageNumber, pageSize);
            return Ok(result);
        }

        #endregion

        #region Post Endpoints

        /// <summary>
        /// Add a review for an attraction
        /// </summary>
        [HttpPost("attraction/{attractionId}")]
        [Authorize]
        public async Task<IActionResult> AddAttractionReview(int attractionId, [FromBody] ReviewRequestDto reviewDto)
        {
            _logger.LogInformation("Adding review for attraction ID: {AttractionId}", attractionId);
            
            var addReviewDto = new AddReviewDto
            {
                Rating = reviewDto.Rating,
                Comment = reviewDto.Comment,
                EntityId = attractionId,
                ReviewType = ReviewType.Attraction
            };

            var result = await _reviewService.AddReviewAsync(addReviewDto);
            return CreatedAtAction(nameof(GetReviewById), new { id = result.Data?.Id, reviewType = ReviewType.Attraction }, result);
        }

        /// <summary>
        /// Add a review for a restaurant
        /// </summary>
        [HttpPost("restaurant/{restaurantId}")]
        [Authorize]
        public async Task<IActionResult> AddRestaurantReview(int restaurantId, [FromBody] ReviewRequestDto reviewDto)
        {
            _logger.LogInformation("Adding review for restaurant ID: {RestaurantId}", restaurantId);
            
            var addReviewDto = new AddReviewDto
            {
                Rating = reviewDto.Rating,
                Comment = reviewDto.Comment,
                EntityId = restaurantId,
                ReviewType = ReviewType.Restaurant
            };
            
            var result = await _reviewService.AddReviewAsync(addReviewDto);
            return CreatedAtAction(nameof(GetReviewById), new { id = result.Data?.Id, reviewType = ReviewType.Restaurant }, result);
        }

        /// <summary>
        /// Add a review for a hotel
        /// </summary>
        [HttpPost("hotel/{hotelId}")]
        [Authorize]
        public async Task<IActionResult> AddHotelReview(int hotelId, [FromBody] ReviewRequestDto reviewDto)
        {
            _logger.LogInformation("Adding review for hotel ID: {HotelId}", hotelId);
            
            var addReviewDto = new AddReviewDto
            {
                Rating = reviewDto.Rating,
                Comment = reviewDto.Comment,
                EntityId = hotelId,
                ReviewType = ReviewType.Hotel
            };
            
            var result = await _reviewService.AddReviewAsync(addReviewDto);
            return CreatedAtAction(nameof(GetReviewById), new { id = result.Data?.Id, reviewType = ReviewType.Hotel }, result);
        }

        #endregion

        #region Put Endpoints

        /// <summary>
        /// Update an existing review
        /// </summary>
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateReview(int id, [FromBody] UpdateReviewDto updateReviewDto)
        {
            _logger.LogInformation("Updating review with ID: {ReviewId}", id);
            
            updateReviewDto.Id = id;
            var result = await _reviewService.UpdateReviewAsync(id, updateReviewDto);
            return Ok(result);
        }

        #endregion

        #region Delete Endpoints

        /// <summary>
        /// Delete a review
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteReview(int id)
        {
            _logger.LogInformation("Deleting review with ID: {ReviewId}", id);
            
            var result = await _reviewService.DeleteReviewAsync(id);
            return Ok(result);
        }

        #endregion
    }
}
