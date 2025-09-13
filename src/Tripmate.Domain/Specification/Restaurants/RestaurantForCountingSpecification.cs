using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tripmate.Domain.Entities.Models;

namespace Tripmate.Domain.Specification.Restaurants
{
    public class RestaurantForCountingSpecification:BaseSpecification<Restaurant,int>
    {
        public RestaurantForCountingSpecification(RestaurantsParameters parameters)
            :base(x => (string.IsNullOrEmpty(parameters.Search) ||
             x.Name.ToLower().Contains(parameters.Search.ToLowerInvariant())))
        {
        }
    }
}
