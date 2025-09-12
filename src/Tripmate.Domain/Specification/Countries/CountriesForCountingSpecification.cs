using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tripmate.Domain.Entities.Models;

namespace Tripmate.Domain.Specification.Countries
{
    public class CountriesForCountingSpecification :BaseSpecification<Country,int>
    {
        public CountriesForCountingSpecification( CountryParameters parameters)
            :base(x => (string.IsNullOrEmpty(parameters.Search) ||
             x.Name.ToLower().Contains(parameters.Search.ToLowerInvariant())))
        {
        }
    }
}
