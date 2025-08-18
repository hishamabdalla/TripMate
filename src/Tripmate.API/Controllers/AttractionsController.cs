using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tripmate.Application.Services.Abstractions.Attraction;
using Tripmate.Application.Services.Attractions.DTOs;

namespace Tripmate.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttractionsController : ControllerBase
    {
        private readonly IAttractionService _attractionService;

        public AttractionsController(IAttractionService attractionService)
        {

            _attractionService = attractionService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAtrractions()
        {
            var result = await _attractionService.GetAllAttractionsAsync();

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAttractionById(int id)
        {
            var result = await _attractionService.GetAttractionByIdAsync(id);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> AddAttraction([FromForm] SetAttractionDto setAttractionDto)
        {
            var result = await _attractionService.AddAsync(setAttractionDto);

            return CreatedAtAction(nameof(GetAttractionById), new { id = result.Data.Id }, result);
        }




    }
}
