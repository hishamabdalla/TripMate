using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tripmate.Domain.Entities.Models;

namespace Tripmate.Domain.Specification.Hotels
{
    public class HotelsSpecification:BaseSpecification<Hotel,int>
    {
        public HotelsSpecification(HotelsParameters parameters) : base(x => (string.IsNullOrEmpty(parameters.Search) || x.Name.ToLower().Contains(parameters.Search.ToLowerInvariant())))
        {
            ApplyInclude();
            ApplyPaging(parameters.PageNumber, parameters.PageSize);
        }
        public HotelsSpecification()
        {
            ApplyInclude();
        }
        public HotelsSpecification(int id) : base(x => x.Id == id)
        {
            ApplyInclude();
        }
        public HotelsSpecification(int regionId, bool byRegion)
        : base(x => x.RegionId == regionId)
        {
            ApplyInclude();
        }
        private void ApplyInclude()
        {
            AddInclude(x => x.Region);
        }
    }
}
