using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tripmate.Application.Services.Restaurants.DTOS
{
    public class ReadRestaurantDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Rating { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public string CuisineType { get; set; }
        public int RegionId { get; set; }
    }
}
