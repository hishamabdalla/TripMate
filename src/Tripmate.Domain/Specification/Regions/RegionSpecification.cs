using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tripmate.Domain.Entities.Models;

namespace Tripmate.Domain.Specification.Regions
{
    public class RegionSpecification:BaseSpecification<Region,int>
    {
        public RegionSpecification(RegionParameters parameters) : base(x =>(string.IsNullOrEmpty(parameters.Search) || x.Name.ToLower().Contains(parameters.Search.ToLowerInvariant())))
        {
            ApplyInclude();
            int skip = Math.Max((parameters.PageNumber - 1) * parameters.PageSize, 0);
            ApplyPaging(skip, parameters.PageSize);
        }
        public RegionSpecification(int countryId, bool includeAll=true)
            : base(x => x.CountryId == countryId)
        {
            if (includeAll)
            {
                ApplyInclude();
            }
        }
        public RegionSpecification(int regionId)
            : base(x => x.Id ==regionId)
        {

            ApplyInclude();
            
        }
        private void ApplyInclude()
        {
            AddInclude(x => x.Country);
            AddInclude(x => x.Attractions);
            AddInclude(x => x.Hotels);
            AddInclude(x => x.Restaurants);
        }

    }
}
