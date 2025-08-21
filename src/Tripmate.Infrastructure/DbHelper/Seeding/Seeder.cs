using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tripmate.Infrastructure.Data.Context;
using Tripmate.Infrastructure.DbHelper.Seeding.DataSeeder.Attractions;
using Tripmate.Infrastructure.DbHelper.Seeding.DataSeeder.Countries;
using Tripmate.Infrastructure.DbHelper.Seeding.DataSeeder.Regions;

namespace Tripmate.Infrastructure.DbHelper.Seeding
{
    public class Seeder:ISeeder
    {
        private readonly TripmateDbContext _context;
        public Seeder(TripmateDbContext context)
        {
            _context = context;
        }

        public async Task SeedAsync()
        {
            if(!_context.Countries.Any())
            {
                var countries =CountriesSeeder.GetCountries();
                if (countries != null)
                {
                    _context.Countries.AddRange(countries);
                   await _context.SaveChangesAsync();
                }
            }
            if (!_context.Regions.Any())
            {
                var regions = RegionsSeeder.GetRegions();
                if (regions != null)
                {
                    _context.Regions.AddRange(regions);
                    await _context.SaveChangesAsync();
                }
            }

            if (!_context.Attractions.Any())
            {
                var attractions = AttractionsSeeder.GetAttractions();
                if (attractions != null)
                {
                    _context.Attractions.AddRange(attractions);
                    await _context.SaveChangesAsync();
                }

            }
           
        }
    }
}
