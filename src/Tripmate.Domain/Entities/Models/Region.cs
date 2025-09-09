using System.ComponentModel.DataAnnotations.Schema;
using Tripmate.Domain.Entities.Base;

namespace Tripmate.Domain.Entities.Models
{
    public class Region:BaseEntity<int>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        [ForeignKey("Country")]
        public int CountryId { get; set; }
        public Country Country { get; set; }
        public ICollection<Attraction> Attractions { get; set; } = new HashSet<Attraction>();
        public ICollection<Hotel> Hotels { get; set; } = new HashSet<Hotel>();
        public ICollection<Restaurant> Restaurants { get; set; } = new HashSet<Restaurant>();
    }
}
