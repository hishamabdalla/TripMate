using System.Text.Json;
using Tripmate.Domain.Entities.Models;
using Tripmate.Domain.Exceptions;

namespace Tripmate.Infrastructure.DbHelper.Seeding.DataSeeder.Countries
{
    public class CountriesSeeder
    {
        public static List<Country> GetCountries()
        {

            var filePath = @"..\Tripmate.Infrastructure\DbHelper\Seeding\DataSeeder\Countries\Counties.json";
            if (!File.Exists(filePath))
            {
                throw new BadRequestException($"The file {filePath} does not exist.");
            }
            var json = File.ReadAllText(filePath);

            var countries = JsonSerializer.Deserialize<List<Country>>(json);
            if (countries == null)
            {
                throw new BadRequestException("Failed to deserialize countries from JSON.");

            }

            return countries;

        }
    }
}
