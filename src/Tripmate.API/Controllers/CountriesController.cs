using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Tripmate.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountriesController : ControllerBase
    {
        [Authorize]
        [HttpGet]
        public IActionResult GetAllCountries()
        {
            // Logic to retrieve all countries would go here
            return Ok("List of countries would be returned here.");
        }

    }
}
