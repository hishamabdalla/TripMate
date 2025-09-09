using Tripmate.Domain.Entities.Models;

namespace Tripmate.Domain.Specification.Regions
{
    public class RegionSpecification:BaseSpecification<Region,int>
    {
        public RegionSpecification(RegionParameters parameters) : base(x =>(string.IsNullOrEmpty(parameters.Search) || x.Name.ToLower().Contains(parameters.Search.ToLowerInvariant())))
        {
            ApplyInclude();
            ApplyPaging(parameters.PageNumber, parameters.PageSize);
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
