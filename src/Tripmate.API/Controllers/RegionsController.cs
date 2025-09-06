using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tripmate.Application.Services.Abstractions.Region;
using Tripmate.Application.Services.Regions.DTOs;
using Tripmate.Domain.Specification.Regions;

namespace Tripmate.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegionsController : ControllerBase
    {
        private readonly IRegionService _regionService;

        public RegionsController(IRegionService regionService)
        {
            _regionService = regionService;
        }

        [HttpGet("County/{countryId}/")]
        public async Task<IActionResult> GetAllRegionsForCountry(int countryId)
        {
            var result = await _regionService.GetAllRegionForCountryAsync(countryId);
            return Ok(result);
        }

        [HttpGet("{regionId}")]

        public async Task<IActionResult> GetRegionByIdForCountry(int regionId)
        {
            var result = await _regionService.GetRegionByIdAsync(regionId);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> AddRegion([FromForm]SetRegionDto setRegionDto)
        {
            var result = await _regionService.CreateRegionAsync(setRegionDto);
            return CreatedAtAction(nameof(GetRegionByIdForCountry), new { regionId = result.Data?.Id }, result);
        }

        [HttpPut("{regionId}")]
        public async Task<IActionResult> UpdateRegion(int regionId, [FromForm]SetRegionDto setRegionDto)
        {
            var result = await _regionService.UpdateRegionAsync(regionId, setRegionDto);
            return Ok(result);
        }

        [HttpDelete("{regionId}")]
        public async Task<IActionResult> DeleteRegion(int regionId)
        {
            var result = await _regionService.DeleteRegionAsync(regionId);
            return Ok(result);

        }
        [HttpGet("GetAllRegions")]
        public async Task<IActionResult> GetAllRegions([FromQuery] RegionParameters parameters)
        {
            var result = await _regionService.GetRegionsAsync(parameters);
            return Ok(result);
        }
    }
    
}
