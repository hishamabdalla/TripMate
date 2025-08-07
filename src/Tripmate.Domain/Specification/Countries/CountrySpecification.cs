using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tripmate.Domain.Entities.Base;
using Tripmate.Domain.Entities.Models;

namespace Tripmate.Domain.Specification.Countries
{
    public class CountrySpecification:BaseSpecification<Country, int>
    {
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
