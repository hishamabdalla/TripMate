using Tripmate.Domain.Entities.Base;
using Tripmate.Domain.Interfaces;

namespace Tripmate.Domain.Entities.Models
{
    public class Country : BaseEntity<int>,IHasImageUrl
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public ICollection<Region> Region { get; set; } = new HashSet<Region>();
    }
}
