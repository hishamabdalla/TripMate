using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tripmate.Domain.Entities.Models;

namespace Tripmate.Domain.Specification.Attractions
{
    public class AttractionsForCountingSpecification:BaseSpecification<Attraction,int>
    {
        public AttractionsForCountingSpecification(AttractionParameter parameter)
            :base(x => (string.IsNullOrEmpty(parameter.Search) ||
             x.Name.ToLower().Contains(parameter.Search.ToLowerInvariant())))
        { }
    }
}
