using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Tripmate.API.Attributes;
using Tripmate.Application.Services.Abstractions.Region;
using Tripmate.Application.Services.Regions.DTOs;
using Tripmate.Domain.Specification.Regions;

namespace Tripmate.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // All endpoints require authentication
    public class RegionsController : ControllerBase
    {
        private readonly IRegionService _regionService;
        private readonly ILogger<RegionsController> _logger;

        public RegionsController(IRegionService regionService, ILogger<RegionsController> logger)
        {
            _regionService = regionService;
            _logger = logger;
        }

        [HttpGet("County/{countryId}/")]
        [Cached(1)]
        [AllowAnonymous] // Public endpoint - anyone can view regions
        public async Task<IActionResult> GetAllRegionsForCountry(int countryId)
        {
            _logger.LogInformation("Getting all regions for country ID: {CountryId}", countryId);
            var result = await _regionService.GetAllRegionForCountryAsync(countryId);
            _logger.LogInformation("Successfully retrieved regions for country ID: {CountryId}", countryId);
            return Ok(result);
        }

        [HttpGet("{regionId}")]
        [AllowAnonymous] // Public endpoint - anyone can view a region
        public async Task<IActionResult> GetRegionByIdForCountry(int regionId)
        {
            _logger.LogInformation("Getting region by ID: {RegionId}", regionId);
            var result = await _regionService.GetRegionByIdAsync(regionId);
            _logger.LogInformation("Successfully retrieved region with ID: {RegionId}", regionId);
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")] // Only Admin can add regions
        public async Task<IActionResult> AddRegion([FromForm] SetRegionDto setRegionDto)
        {
            _logger.LogInformation("Adding new region: {RegionName} for country ID: {CountryId}", setRegionDto.Name, setRegionDto.CountryId);
            var result = await _regionService.CreateRegionAsync(setRegionDto);
            _logger.LogInformation("Successfully added region: {RegionName} with ID: {RegionId}", setRegionDto.Name, result.Data?.Id);
            return CreatedAtAction(nameof(GetRegionByIdForCountry), new { regionId = result.Data?.Id }, result);
        }

        [HttpPut("{regionId}")]
        [Authorize(Roles = "Admin")] // Only Admin can update regions
        public async Task<IActionResult> UpdateRegion(int regionId, [FromForm] SetRegionDto setRegionDto)
        {
            _logger.LogInformation("Updating region with ID: {RegionId}", regionId);
            var result = await _regionService.UpdateRegionAsync(regionId, setRegionDto);
            _logger.LogInformation("Successfully updated region with ID: {RegionId}", regionId);
            return Ok(result);
        }

        [HttpDelete("{regionId}")]
        [Authorize(Roles = "Admin")] // Only Admin can delete regions
        public async Task<IActionResult> DeleteRegion(int regionId)
        {
            _logger.LogInformation("Deleting region with ID: {RegionId}", regionId);
            var result = await _regionService.DeleteRegionAsync(regionId);
            _logger.LogInformation("Successfully deleted region with ID: {RegionId}", regionId);
            return Ok(result);
        }

        [HttpGet("GetAllRegions")]
        [AllowAnonymous] // Public endpoint - anyone can view all regions
        public async Task<IActionResult> GetAllRegions([FromQuery] RegionParameters parameters)
        {
            _logger.LogInformation("Getting all regions with parameters: PageNumber={PageNumber}, PageSize={PageSize}", parameters.PageNumber, parameters.PageSize);
            var result = await _regionService.GetRegionsAsync(parameters);
            _logger.LogInformation("Successfully retrieved {Count} regions", result.Data?.Count() ?? 0);
            return Ok(result);
        }
    }
}
