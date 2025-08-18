using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tripmate.Domain.Entities.Models;

namespace Tripmate.Application.Services.Attractions.DTOs
{
    public class AttractionDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public string Type { get; set; } = string.Empty;
        public DateTime OpeningHours { get; set; }
        public string TicketPrice { get; set; }      
    }
}
