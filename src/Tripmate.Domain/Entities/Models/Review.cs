using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Tripmate.Domain.AppSettings;
using Tripmate.Domain.Entities.Base;

namespace Tripmate.Domain.Entities.Models
{
    public class Review : BaseEntity<int>
    {
        public int Rating { get; set; }
        public string Comment { get; set; }
        public DateTime ReviewDate { get; set; }

        // Foreign Keys for different entity types
        public int? AttractionId { get; set; }
        public int? RestaurantId { get; set; }
        public int? HotelId { get; set; }
        public string UserId { get; set; }


        // Navigation properties
        public ApplicationUser User { get; set; }
        public virtual Attraction Attraction { get; set; }
        public virtual Restaurant Restaurant { get; set; }
        public virtual Hotel Hotel { get; set; }

    
    }
}
