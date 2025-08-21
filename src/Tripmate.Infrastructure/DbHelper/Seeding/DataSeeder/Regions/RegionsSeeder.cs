using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Tripmate.Domain.Entities.Models;
using Tripmate.Domain.Exceptions;

namespace Tripmate.Infrastructure.DbHelper.Seeding.DataSeeder.Regions
{
    public class RegionsSeeder
    {
        public static List<Region> GetRegions()
        {
            var filePath = @"..\Tripmate.Infrastructure\DbHelper\Seeding\DataSeeder\Regions\Regions.json";
            if (!File.Exists(filePath))
            {
                throw new BadRequestException($"The file {filePath} does not exist.");
            }
            var json = File.ReadAllText(filePath);
            var regions = JsonSerializer.Deserialize<List<Region>>(json);
            if (regions == null)
            {
                throw new BadRequestException("Failed to deserialize regions from JSON.");
            }
            return regions;
        }
    }
}
