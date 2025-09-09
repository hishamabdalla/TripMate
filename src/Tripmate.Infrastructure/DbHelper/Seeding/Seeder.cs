using Tripmate.Infrastructure.Data.Context;
using Tripmate.Infrastructure.DbHelper.Seeding.DataSeeder.Attractions;
using Tripmate.Infrastructure.DbHelper.Seeding.DataSeeder.Countries;
using Tripmate.Infrastructure.DbHelper.Seeding.DataSeeder.Regions;

namespace Tripmate.Infrastructure.DbHelper.Seeding
{
    public class Seeder(TripmateDbContext context) : ISeeder
    {
        public async Task SeedAsync()
        {
            if(!context.Countries.Any())
            {
                var countries =CountriesSeeder.GetCountries();
                if (countries != null)
                {
                    context.Countries.AddRange(countries);
                   await context.SaveChangesAsync();
                }
            }
            if (!context.Regions.Any())
            {
                var regions = RegionsSeeder.GetRegions();
                if (regions != null)
                {
                    context.Regions.AddRange(regions);
                    await context.SaveChangesAsync();
                }
            }

            if (!context.Attractions.Any())
            {
                var attractions = AttractionsSeeder.GetAttractions();
                if (attractions != null)
                {
                    context.Attractions.AddRange(attractions);
                    await context.SaveChangesAsync();
                }

            }
           
        }
    }
}
