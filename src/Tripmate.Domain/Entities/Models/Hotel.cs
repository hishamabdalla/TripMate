using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Tripmate.Domain.Entities.Base;

namespace Tripmate.Domain.Entities.Models
{
    public class Hotel :BaseEntity<int>
    {
        public string Name { get; set; }
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
        public int Stars { get; set; }
        public string PricePerNight { get; set; }
        public string ImageUrl { get; set; }
        [ForeignKey("Region")]
        public int RegionId { get; set; }
        public Region Region { get; set; }
    }
}
