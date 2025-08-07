using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tripmate.Domain.Entities.Base;

namespace Tripmate.Domain.Entities.Models
{
    public class Restaurant : BaseEntity<int>
    {
        public string Name { get; set; }
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
        public int Rating { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public string CuisineType { get; set; }
        [ForeignKey("Region")]
        public int RegionId { get; set; }
        public Region Region { get; set; }
    }
}
