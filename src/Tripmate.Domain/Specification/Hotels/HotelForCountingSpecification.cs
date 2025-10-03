using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tripmate.Domain.Entities.Models;
using Tripmate.Domain.Specification.Restaurants;

namespace Tripmate.Domain.Specification.Hotels
{
    public class HotelForCountingSpecification:BaseSpecification<Hotel,int>
    {
        public HotelForCountingSpecification(HotelsParameters parameters)
            : base(x => (string.IsNullOrEmpty(parameters.Search) ||
             x.Name.ToLower().Contains(parameters.Search.ToLowerInvariant())))
        {
        }
    }
}
