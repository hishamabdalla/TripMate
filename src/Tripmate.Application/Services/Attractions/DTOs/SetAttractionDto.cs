using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tripmate.Domain.Entities.Models;

namespace Tripmate.Application.Services.Attractions.DTOs
{
    public class SetAttractionDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public IFormFile? ImageUrl { get; set; }
        public string Type { get; set; } = string.Empty;
        public string OpeningHours { get; set; }
        public string TicketPrice { get; set; }
        public int RegionId { get; set; }

    }
}