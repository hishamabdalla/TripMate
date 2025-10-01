using Microsoft.AspNetCore.Mvc;
using Tripmate.API.Attributes;
using Tripmate.Application.Services.Abstractions.Attraction;
using Tripmate.Application.Services.Attractions.DTOs;
using Tripmate.Domain.Specification.Attractions;

namespace Tripmate.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttractionsController(IAttractionService attractionService) : ControllerBase
    {
        [HttpGet("GetAtrractions")]
        [Cached(1)]
        public async Task<IActionResult> GetAtrractions([FromQuery] AttractionParameter parameter)
        {
            var result = await attractionService.GetAttractionsAsync(parameter);

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAttractionById(int id)
        {
            var result = await attractionService.GetAttractionByIdAsync(id);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> AddAttraction([FromForm] SetAttractionDto setAttractionDto)
        {
            var result = await attractionService.AddAsync(setAttractionDto);

            return CreatedAtAction(nameof(GetAttractionById), new { id = result.Data.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAttraction(int id, [FromForm] SetAttractionDto setAttractionDto)
        {

            var result = await attractionService.UpdateAsync(id, setAttractionDto);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAttraction(int id)
        {
            var result = await attractionService.Delete(id);

            return Ok(result);

        }
    }
}
