using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tripmate.Domain.Entities.Models;

namespace Tripmate.Domain.Specification.Regions
{
    public class RegionForCountingSpecification : BaseSpecification<Region,int>
    {
        public RegionForCountingSpecification(RegionParameters parameters)
        : base(x => (string.IsNullOrEmpty(parameters.Search) ||
             x.Name.ToLower().Contains(parameters.Search.ToLowerInvariant())))
        {
        }
    }
}
