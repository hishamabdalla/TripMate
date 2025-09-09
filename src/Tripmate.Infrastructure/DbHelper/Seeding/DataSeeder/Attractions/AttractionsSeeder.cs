using System.Text.Json;
using Tripmate.Domain.Entities.Models;
using Tripmate.Domain.Exceptions;

namespace Tripmate.Infrastructure.DbHelper.Seeding.DataSeeder.Attractions
{
    public class AttractionsSeeder
    {
        public static List<Attraction> GetAttractions()
        {
            var filePath = @"..\Tripmate.Infrastructure\DbHelper\Seeding\DataSeeder\Attractions\Attractions.json";
            if (!File.Exists(filePath))
            {
                throw new BadRequestException($"The file {filePath} does not exist.");
            }
            var json = File.ReadAllText(filePath);
            var attractions = JsonSerializer.Deserialize<List<Attraction>>(json);
            if (attractions == null)
            {
                throw new BadRequestException("Failed to deserialize attractions from JSON.");
            }
            return attractions;
        }
    }
}
