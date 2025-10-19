using Microsoft.AspNetCore.Http;
using Tripmate.Domain.Enums;

namespace Tripmate.Application.Services.Attractions.DTOs
{
    public class SetAttractionDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public IFormFile? ImageUrl { get; set; }
        public AttractionType Type { get; set; } 
        public string OpeningHours { get; set; }
        public string TicketPrice { get; set; }
        public int RegionId { get; set; }

    }
}