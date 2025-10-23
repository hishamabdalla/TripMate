using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tripmate.Domain.Entities.Base;
using Tripmate.Domain.Enums;

namespace Tripmate.Domain.Entities.Models
{
    public class Favorite : BaseEntity<int>
    {
        public FavoriteType FavoriteType { get; set; }

        [ForeignKey("User")]
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        [ForeignKey("Hotel")]
        public int? HotelId { get; set; }
        public Hotel Hotel { get; set; }

        [ForeignKey("Restaurant")]
        public int? RestaurantId { get; set; }
        public Restaurant Restaurant { get; set; }

        [ForeignKey("Attraction")]
        public int? AttractionId { get; set; }
        public Attraction Attraction { get; set; }

    }
}
