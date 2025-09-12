using Microsoft.AspNetCore.Mvc;
using Tripmate.Application.Services.Abstractions.Country;
using Tripmate.Application.Services.Countries.DTOs;
using Tripmate.Domain.Specification.Countries;
using Tripmate.Domain.Specification.Regions;

namespace Tripmate.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountriesController(ICountryService countryService) : ControllerBase
    {
        [HttpGet("GetCountries")]
        public async Task<IActionResult> GetCountries([FromQuery] CountryParameters parameters)
        {
            var response = await countryService.GetCountriesAsync(parameters);
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCountryById(int id)
        {
            var response = await countryService.GetCountryByIdAsync(id);
            if (!response.Success)
            {
                return NotFound(response);
            }
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> AddCountry([FromForm] SetCountryDto setCountryDto)
        {

            var response = await countryService.AddAsync(setCountryDto);
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return CreatedAtAction(nameof(GetCountryById), new { id = response.Data.Id }, response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCountry(int id, [FromForm] SetCountryDto countryDto)
        {
            var response = await countryService.Update(id, countryDto);
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCountry(int id)
        {
            var response = await countryService.Delete(id);
            if (!response.Success)
            {
                return NotFound(response);
            }
            return NoContent();

        }
    }
}
