using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tripmate.Infrastructure.DbHelper.Seeding
{
    public interface ISeeder
    {
        Task SeedAsync();
    }
}
