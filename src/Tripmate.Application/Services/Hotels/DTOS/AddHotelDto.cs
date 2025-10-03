using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Tripmate.Application.Services.Hotels.DTOS
{
    public class AddHotelDto
    {
        public string Name { get; set; }
        public int? Stars { get; set; }
        public string PricePerNight { get; set; }
        public IFormFile ImageUrl { get; set; }
        public int RegionId { get; set; }
    }
}
