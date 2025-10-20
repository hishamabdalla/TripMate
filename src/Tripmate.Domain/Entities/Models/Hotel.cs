using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Tripmate.Domain.Entities.Base;
using Tripmate.Domain.Interfaces;

namespace Tripmate.Domain.Entities.Models
{
    public class Hotel :BaseEntity<int>,IHasImageUrl
    {
        public string Name { get; set; }
        public string PricePerNight { get; set; }
        public string ImageUrl { get; set; }
        [ForeignKey("Region")]
        public int RegionId { get; set; }
        public Region Region { get; set; }
    }
}
