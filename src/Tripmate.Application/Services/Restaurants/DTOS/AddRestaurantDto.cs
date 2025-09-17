using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Tripmate.Application.Services.Restaurants.DTOS
{
    public class AddRestaurantDto
    {
        public string Name { get; set; }
        public int Rating { get; set; }
        public string Description { get; set; }
        public IFormFile ImageUrl { get; set; }
        public string CuisineType { get; set; }
        public int RegionId { get; set; }
    }
}
