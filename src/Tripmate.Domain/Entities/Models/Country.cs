using Tripmate.Domain.Entities.Base;

namespace Tripmate.Domain.Entities.Models
{
    public class Country :BaseEntity<int>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public ICollection<Region> Region { get; set; } = new HashSet<Region>();
    }
}
