using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Tripmate.Application.Services.Abstractions.Attraction;
using Tripmate.Application.Services.Abstractions.Restaurant;
using Tripmate.Application.Services.Attractions;
using Tripmate.Application.Services.Attractions.DTOs;
using Tripmate.Application.Services.Restaurants.DTOS;
using Tripmate.Domain.Specification.Attractions;
using Tripmate.Domain.Specification.Restaurants;

namespace Tripmate.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // All endpoints require authentication
    public class RestaurantsController : ControllerBase
    {
        private readonly IRestaurantService _restaurantService;
        private readonly ILogger<RestaurantsController> _logger;
        
        public RestaurantsController(IRestaurantService restaurantService, ILogger<RestaurantsController> logger)
        {
            _restaurantService = restaurantService;
            _logger = logger;
        }
        
        [HttpGet("GetRestaurants")]
        [AllowAnonymous] // Public endpoint - anyone can view restaurants
        public async Task<IActionResult> GetRestaurants([FromQuery] RestaurantsParameters parameter)
        {
            _logger.LogInformation("Getting restaurants with parameters: PageNumber={PageNumber}, PageSize={PageSize}", 
                parameter.PageNumber, parameter.PageSize);
            
            var result = await _restaurantService.GetRestaurantsAsync(parameter);
            
            _logger.LogInformation("Successfully retrieved {Count} restaurants", result.Data?.Count() ?? 0);
            return Ok(result);
        }
        
        [HttpGet("GetRestaurantById/{id}")]
        [AllowAnonymous] // Public endpoint - anyone can view a restaurant
        public async Task<IActionResult> GetRestaurantById(int id)
        {
            _logger.LogInformation("Getting restaurant by ID: {RestaurantId}", id);
            
            var result = await _restaurantService.GetRestaurantByIdAsync(id);
            
            _logger.LogInformation("Successfully retrieved restaurant with ID: {RestaurantId}", id);
            return Ok(result);
        }
        
        [HttpGet("GetRestaurantByRegionId")]
        [AllowAnonymous] // Public endpoint - anyone can view restaurants by region
        public async Task<IActionResult> GetRestaurantByRegionId(int id)
        {
            _logger.LogInformation("Getting restaurants by region ID: {RegionId}", id);
            
            var result = await _restaurantService.GetRestaurantByRegionIdAsync(id);
            
            _logger.LogInformation("Successfully retrieved restaurants for region ID: {RegionId}", id);
            return Ok(result);
        }
        
        [HttpPost("CreateRestaurant")]
        [Authorize(Roles = "Admin")] // Only Admin can add restaurants
        public async Task<IActionResult> AddRestaurant([FromForm] AddRestaurantDto addRestaurantDto)
        {
            _logger.LogInformation("Adding new restaurant: {RestaurantName}", addRestaurantDto.Name);
            
            var result = await _restaurantService.AddRestaurantAsync(addRestaurantDto);
            
            _logger.LogInformation("Successfully added restaurant: {RestaurantName} with ID: {RestaurantId}", 
                addRestaurantDto.Name, result.Data?.Id);
            return Ok(result);
        }

        [HttpPut("UpdateRestaurant")]
        [Authorize(Roles = "Admin")] // Only Admin can update restaurants
        public async Task<IActionResult> UpdateRestaurant([FromForm] UpdateRestaurantDto updateRestaurantDto)
        {
            _logger.LogInformation("Updating restaurant with ID: {RestaurantId}", updateRestaurantDto.Id);
            
            var result = await _restaurantService.UpdateRestaurantAsync(updateRestaurantDto);
            
            _logger.LogInformation("Successfully updated restaurant with ID: {RestaurantId}", updateRestaurantDto.Id);
            return Ok(result);
        }

        [HttpDelete("DeleteRestaurant")]
        [Authorize(Roles = "Admin")] // Only Admin can delete restaurants
        public async Task<IActionResult> DeleteRestaurant(int id)
        {
            _logger.LogInformation("Deleting restaurant with ID: {RestaurantId}", id);
            
            var result = await _restaurantService.DeleteRestaurant(id);
            
            _logger.LogInformation("Successfully deleted restaurant with ID: {RestaurantId}", id);
            return Ok(result);
        }
    }
}
