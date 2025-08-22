using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tripmate.Domain.Entities.Base;
using Tripmate.Domain.Enums;

namespace Tripmate.Domain.Entities.Models
{
    public class Attraction : BaseEntity<int>
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
