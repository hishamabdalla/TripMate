using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tripmate.Application.Services.Hotels.DTOS
{
    public class ReadHotelDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Stars { get; set; }
        public string PricePerNight { get; set; }
        public string ImageUrl { get; set; }
        public int RegionId { get; set; }
    }
}
