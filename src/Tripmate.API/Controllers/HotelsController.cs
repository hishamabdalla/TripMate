using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Tripmate.Application.Services.Abstractions.Hotel;
using Tripmate.Application.Services.Hotels.DTOS;
using Tripmate.Application.Services.Restaurants.DTOS;
using Tripmate.Domain.Specification.Hotels;
using Tripmate.Domain.Specification.Restaurants;

namespace Tripmate.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // All endpoints require authentication
    public class HotelsController : ControllerBase
    {
        private readonly IHotelServices _hotelServices;
        private readonly ILogger<HotelsController> _logger;
        
        public HotelsController(IHotelServices hotelServices, ILogger<HotelsController> logger)
        {
            _hotelServices = hotelServices;
            _logger = logger;
        }
        
        [HttpGet("GetHotels")]
        [AllowAnonymous] // Public endpoint - anyone can view hotels
        public async Task<IActionResult> GetHotels([FromQuery] HotelsParameters parameter)
        {
            _logger.LogInformation("Getting hotels with parameters: PageNumber={PageNumber}, PageSize={PageSize}", 
                parameter.PageNumber, parameter.PageSize);
            
            var hotels = await _hotelServices.GetHotelsAsync(parameter);
            
            _logger.LogInformation("Successfully retrieved {Count} hotels", hotels.Data?.Count() ?? 0);
            return Ok(hotels);
        }
        
        [HttpGet("GetHotelById/{id}")]
        [AllowAnonymous] // Public endpoint - anyone can view a hotel
        public async Task<IActionResult> GetHotelById(int id)
        {
            _logger.LogInformation("Getting hotel by ID: {HotelId}", id);
            
            var hotel = await _hotelServices.GetHotelByIdAsync(id);
            
            _logger.LogInformation("Successfully retrieved hotel with ID: {HotelId}", id);
            return Ok(hotel);
        }
        
        [HttpGet("GetHotelsByRegionId")]
        [AllowAnonymous] // Public endpoint - anyone can view hotels by region
        public async Task<IActionResult> GetHotelsByRegionId(int id)
        {
            _logger.LogInformation("Getting hotels by region ID: {RegionId}", id);
            
            var result = await _hotelServices.GetHotelsByRegionIdAsync(id);
            
            _logger.LogInformation("Successfully retrieved hotels for region ID: {RegionId}", id);
            return Ok(result);
        }
        
        [HttpPost("AddHotel")]
        [Authorize(Roles = "Admin")] // Only Admin can add hotels
        public async Task<IActionResult> AddHotel([FromForm] AddHotelDto addHotelDto)
        {
            _logger.LogInformation("Adding new hotel: {HotelName}", addHotelDto.Name);
            
            var result = await _hotelServices.AddHotelAsync(addHotelDto);
            
            _logger.LogInformation("Successfully added hotel: {HotelName} with ID: {HotelId}", 
                addHotelDto.Name, result.Data?.Id);
            return Ok(result);
        }
        
        [HttpPut("UpdateHotel")]
        [Authorize(Roles = "Admin")] // Only Admin can update hotels
        public async Task<IActionResult> UpdateHotel([FromForm] UpdateHotelDto updateHotelDto)
        {
            _logger.LogInformation("Updating hotel with ID: {HotelId}", updateHotelDto.Id);
            
            var result = await _hotelServices.UpdateHotelAsync(updateHotelDto);
            
            _logger.LogInformation("Successfully updated hotel with ID: {HotelId}", updateHotelDto.Id);
            return Ok(result);
        }
        
        [HttpDelete("DeleteHotel")]
        [Authorize(Roles = "Admin")] // Only Admin can delete hotels
        public async Task<IActionResult> DeleteHotel(int id)
        {
            _logger.LogInformation("Deleting hotel with ID: {HotelId}", id);
            
            var result = await _hotelServices.DeleteHotel(id);
            
            _logger.LogInformation("Successfully deleted hotel with ID: {HotelId}", id);
            return Ok(result);
        }
    }
}
