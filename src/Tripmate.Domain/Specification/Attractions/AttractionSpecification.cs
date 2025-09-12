using Tripmate.Domain.Entities.Models;
using Tripmate.Domain.Specification.Countries;

namespace Tripmate.Domain.Specification.Attractions
{
    public class AttractionSpecification : BaseSpecification<Attraction, int>
    {
        public AttractionSpecification(AttractionParameter parameters) : base(x => (string.IsNullOrEmpty(parameters.Search) || x.Name.ToLower().Contains(parameters.Search.ToLowerInvariant())))
        {
            ApplyIncludes();
            ApplyPaging(parameters.PageNumber, parameters.PageSize);
        }
        public AttractionSpecification()
            : base()
        {
            ApplyIncludes();
            AddOrderBy(x => x.Name);
        }

        public AttractionSpecification(int id):base(x => x.Id == id)
        {
            ApplyIncludes();
        }


        private void ApplyIncludes()
        {
            AddInclude(x => x.Reviews);
        }
    }
}
