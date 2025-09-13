using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tripmate.Domain.Entities.Models;
using Tripmate.Domain.Specification.Regions;

namespace Tripmate.Domain.Specification.Restaurants
{
    public class RestaurantSpecification :BaseSpecification<Restaurant, int>
    {
        public RestaurantSpecification(RestaurantsParameters parameters) : base(x => (string.IsNullOrEmpty(parameters.Search) || x.Name.ToLower().Contains(parameters.Search.ToLowerInvariant())))
        {
            ApplyInclude();
            ApplyPaging(parameters.PageNumber, parameters.PageSize);
        }
        public RestaurantSpecification()
        {
            ApplyInclude();
        }
        public RestaurantSpecification(int id) : base(x => x.Id == id)
        {
            ApplyInclude();
        }
        public RestaurantSpecification(int regionId, bool byRegion)
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
