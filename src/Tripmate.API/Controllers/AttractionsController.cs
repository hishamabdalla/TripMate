using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Tripmate.API.Attributes;
using Tripmate.Application.Services.Abstractions.Attraction;
using Tripmate.Application.Services.Attractions.DTOs;
using Tripmate.Domain.Specification.Attractions;

namespace Tripmate.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // All endpoints require authentication
    public class AttractionsController : ControllerBase
    {
        private readonly IAttractionService _attractionService;
        private readonly ILogger<AttractionsController> _logger;

        public AttractionsController(IAttractionService attractionService, ILogger<AttractionsController> logger)
        {
            _attractionService = attractionService;
            _logger = logger;
        }

        [HttpGet("GetAtrractions")]
        [Cached(1)]
        [AllowAnonymous] // Public endpoint - anyone can view attractions
        public async Task<IActionResult> GetAtrractions([FromQuery] AttractionParameter parameter)
        {
            _logger.LogInformation("Getting attractions with parameters: {Parameter}", parameter);

            var result = await _attractionService.GetAttractionsAsync(parameter);

            _logger.LogInformation("Successfully retrieved {Count} attractions", result.Data?.Count() ?? 0);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [AllowAnonymous] // Public endpoint - anyone can view an attraction
        public async Task<IActionResult> GetAttractionById(int id)
        {
            _logger.LogInformation("Getting attraction by ID: {AttractionId}", id);

            var result = await _attractionService.GetAttractionByIdAsync(id);

            _logger.LogInformation("Successfully retrieved attraction with ID: {AttractionId}", id);
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")] // Only Admin can add attractions
        public async Task<IActionResult> AddAttraction([FromForm] SetAttractionDto setAttractionDto)
        {
            _logger.LogInformation("Adding new attraction: {AttractionName}", setAttractionDto.Name);

            var result = await _attractionService.AddAsync(setAttractionDto);

            _logger.LogInformation("Successfully added attraction: {AttractionName} with ID: {AttractionId}", setAttractionDto.Name, result.Data?.Id);
            return CreatedAtAction(nameof(GetAttractionById), new { id = result.Data.Id }, result);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")] // Only Admin can update attractions
        public async Task<IActionResult> UpdateAttraction(int id, [FromForm] SetAttractionDto setAttractionDto)
        {
            _logger.LogInformation("Updating attraction with ID: {AttractionId}", id);

            var result = await _attractionService.UpdateAsync(id, setAttractionDto);

            _logger.LogInformation("Successfully updated attraction with ID: {AttractionId}", id);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")] // Only Admin can delete attractions
        public async Task<IActionResult> DeleteAttraction(int id)
        {
            _logger.LogInformation("Deleting attraction with ID: {AttractionId}", id);

            var result = await _attractionService.DeleteAsync(id);

            _logger.LogInformation("Successfully deleted attraction with ID: {AttractionId}", id);
            return Ok(result);
        }
    }
}
