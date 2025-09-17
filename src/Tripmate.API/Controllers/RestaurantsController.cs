using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
    public class RestaurantsController : ControllerBase
    {
        private readonly IRestaurantService _restaurantService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public RestaurantsController(IRestaurantService restaurantService, IWebHostEnvironment webHostEnvironment)
        {
            _restaurantService=restaurantService;
            _webHostEnvironment=webHostEnvironment;
        }
        [HttpGet("GetRestaurants")]
        public async Task<IActionResult> GetRestaurants([FromQuery] RestaurantsParameters parameter)
        {
            var result = await _restaurantService.GetRestaurantsAsync(parameter);
            return Ok(result);
        }
        [HttpGet("GetRestaurantById/{id}")]
        public async Task<IActionResult> GetRestaurantById(int id)
        {
            var result = await _restaurantService.GetRestaurantByIdAsync(id);
            return Ok(result);
        }
        [HttpGet("GetRestaurantByRegionId")]
        public async Task<IActionResult> GetRestaurantByRegionId(int id)
        {
            var result = await _restaurantService.GetRestaurantByRegionIdAsync(id);
            return Ok(result);
        }
        [HttpPost("CreateRestaurant")]
        public async Task<IActionResult> AddRestaurant([FromForm] AddRestaurantDto addRestaurantDto)
        {
            var result = await _restaurantService.AddRestaurantAsync(addRestaurantDto, _webHostEnvironment.WebRootPath);
            return Ok(result);
        }

        [HttpPut("UpdateRestaurant")]
        public async Task<IActionResult> UpdateRestaurant([FromForm] UpdateRestaurantDto updateRestaurantDto)
        {
            var result = await _restaurantService.UpdateRestaurantAsync(updateRestaurantDto, _webHostEnvironment.WebRootPath);
            return Ok(result);
        }

        [HttpDelete("DeleteRestaurant")]
        public async Task<IActionResult> DeleteRestaurant(int id)
        {
            var result = await _restaurantService.DeleteRestaurant(id);
            return Ok(result);
        }
    }
}
