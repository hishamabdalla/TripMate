using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tripmate.Domain.Entities.Models;

namespace Tripmate.Domain.Specification.Attractions
{
    public class AttractionSpecification : BaseSpecification<Attraction, int>
    {
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
