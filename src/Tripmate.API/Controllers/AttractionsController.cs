using Microsoft.AspNetCore.Mvc;
using Tripmate.Application.Services.Abstractions.Attraction;
using Tripmate.Application.Services.Attractions.DTOs;

namespace Tripmate.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttractionsController(IAttractionService attractionService) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAllAtrractions()
        {
            var result = await attractionService.GetAllAttractionsAsync();

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
