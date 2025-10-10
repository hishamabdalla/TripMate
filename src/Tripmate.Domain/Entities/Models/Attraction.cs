using Tripmate.Domain.Entities.Base;
using Tripmate.Domain.Enums;
using Tripmate.Domain.Interfaces;

namespace Tripmate.Domain.Entities.Models
{
    public class Attraction : BaseEntity<int>,IHasImageUrl
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public AttractionType Type { get; set; } 
        public string OpeningHours { get; set; }
        public string TicketPrice { get; set; }
        public int RegionId { get; set; }
        public Region Region { get; set; }
        public ICollection<Review> Reviews { get; set; } = new HashSet<Review>();

    }
}
