using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Tripmate.API.Attributes;
using Tripmate.Application.Services.Abstractions.Country;
using Tripmate.Application.Services.Countries.DTOs;
using Tripmate.Domain.Specification.Countries;
using Tripmate.Domain.Specification.Regions;

namespace Tripmate.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // All endpoints require authentication
    public class CountriesController : ControllerBase
    {
        private readonly ICountryService _countryService;
        private readonly ILogger<CountriesController> _logger;
        
        public CountriesController(ICountryService countryService, ILogger<CountriesController> logger)
        {
            _countryService = countryService;
            _logger = logger;
        }

        [HttpGet("GetCountries")]
        [Cached(1)]
        [AllowAnonymous] // Public endpoint - anyone can view countries
        public async Task<IActionResult> GetCountries([FromQuery] CountryParameters parameters)
        {
            _logger.LogInformation("Getting countries with parameters: PageNumber={PageNumber}, PageSize={PageSize}", 
                parameters.PageNumber, parameters.PageSize);
            
            var response = await _countryService.GetCountriesAsync(parameters);
            
            _logger.LogInformation("Successfully retrieved {Count} countries", response.Data?.Count() ?? 0);
            return Ok(response);
        }

        [HttpGet("{id}")]
        [AllowAnonymous] // Public endpoint - anyone can view a country
        public async Task<IActionResult> GetCountryById(int id)
        {
            _logger.LogInformation("Getting country by ID: {CountryId}", id);
            
            var response = await _countryService.GetCountryByIdAsync(id);
        
            
            _logger.LogInformation("Successfully retrieved country with ID: {CountryId}", id);
            return Ok(response);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")] // Only Admin can add countries
        public async Task<IActionResult> AddCountry([FromForm] SetCountryDto setCountryDto)
        {
            _logger.LogInformation("Adding new country: {CountryName}", setCountryDto.Name);
            
            var response = await _countryService.AddAsync(setCountryDto);
           
            
            _logger.LogInformation("Successfully added country: {CountryName} with ID: {CountryId}", 
                setCountryDto.Name, response.Data?.Id);
            return CreatedAtAction(nameof(GetCountryById), new { id = response.Data.Id }, response);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")] // Only Admin can update countries
        public async Task<IActionResult> UpdateCountry(int id, [FromForm] SetCountryDto countryDto)
        {
            _logger.LogInformation("Updating country with ID: {CountryId}", id);
            
            var response = await _countryService.Update(id, countryDto);
           
            
            _logger.LogInformation("Successfully updated country with ID: {CountryId}", id);
            return Ok(response);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")] // Only Admin can delete countries
        public async Task<IActionResult> DeleteCountry(int id)
        {
            _logger.LogInformation("Deleting country with ID: {CountryId}", id);
            
            var response = await _countryService.Delete(id);
          
            _logger.LogInformation("Successfully deleted country with ID: {CountryId}", id);
            return NoContent();
        }
    }
}
