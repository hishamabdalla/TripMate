using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tripmate.Application.Services.Abstractions.Attraction;

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




    }
}
