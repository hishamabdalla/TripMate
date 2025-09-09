using Microsoft.AspNetCore.Http;

namespace Tripmate.Application.Services.Countries.DTOs
{
    public class SetCountryDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public IFormFile? ImageUrl { get; set; }

    }
}
