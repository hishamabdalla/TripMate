using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tripmate.Application.Services.Abstractions.Hotel;
using Tripmate.Application.Services.Hotels.DTOS;
using Tripmate.Application.Services.Restaurants.DTOS;
using Tripmate.Domain.Specification.Hotels;
using Tripmate.Domain.Specification.Restaurants;

namespace Tripmate.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HotelsController : ControllerBase
    {
        private readonly IHotelServices _hotelServices;
        public HotelsController(IHotelServices hotelServices)
        {
            _hotelServices=hotelServices;
        }
        [HttpGet("GetHotels")]
        public async Task<IActionResult> GetHotels([FromQuery] HotelsParameters parameter)
        {
            var hotels = await _hotelServices.GetHotelsAsync(parameter);
            return Ok(hotels);
        }
        [HttpGet("GetHotelById/{id}")]
        public async Task<IActionResult> GetHotelById(int id)
        {
            var hotel = await _hotelServices.GetHotelByIdAsync(id);
            return Ok(hotel);
        }
        [HttpGet("GetHotelsByRegionId")]
        public async Task<IActionResult> GetHotelsByRegionId(int id)
        {
            var result = await _hotelServices.GetHotelsByRegionIdAsync(id);
            return Ok(result);
        }
        [HttpPost("AddHotel")]
        public async Task<IActionResult> AddHotel([FromForm] AddHotelDto addHotelDto)
        {
            var result = await _hotelServices.AddHotelAsync(addHotelDto);
            return Ok(result);
        }
        [HttpPut("UpdateHotel")]
        public async Task<IActionResult> UpdateHotel([FromForm] UpdateHotelDto updateHotelDto)
        {
            var result = await _hotelServices.UpdateHotelAsync(updateHotelDto);
            return Ok(result);
        }
        [HttpDelete("DeleteHotel")]
        public async Task<IActionResult> DeleteHotel(int id)
        {
            var result = await _hotelServices.DeleteHotel(id);
            return Ok(result);
        }
    }
}
