using Tripmate.Domain.Entities.Models;
using Tripmate.Domain.Specification.Regions;

namespace Tripmate.Domain.Specification.Countries
{
    public class CountrySpecification:BaseSpecification<Country, int>
    {

        public CountrySpecification(CountryParameters parameters) : base(x => (string.IsNullOrEmpty(parameters.Search) || x.Name.ToLower().Contains(parameters.Search.ToLowerInvariant())))
        {
            ApplyInclude();
            ApplyPaging(parameters.PageNumber, parameters.PageSize);
        }
        public CountrySpecification()
        { 
            ApplyInclude();
        }

        public CountrySpecification(int id) : base(x => x.Id == id )
        {
            ApplyInclude();
        }

        private void ApplyInclude()
        {
            AddInclude(x => x.Region);
        }
    }
}
