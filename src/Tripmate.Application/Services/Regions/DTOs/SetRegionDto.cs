using Microsoft.AspNetCore.Http;

namespace Tripmate.Application.Services.Regions.DTOs
{
    public class SetRegionDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public IFormFile? ImageUrl { get; set; }
        public int CountryId { get; set; }
    }
}
