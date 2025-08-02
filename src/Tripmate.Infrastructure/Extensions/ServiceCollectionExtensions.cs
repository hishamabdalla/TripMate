using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tripmate.Domain.Entities;
using Tripmate.Infrastructure.Data.Context;

namespace Tripmate.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services,IConfiguration configuration)
        {

            // Add DbContext services
            services.AddDbContextServices(configuration);

            // Add Identity services
            services.AddIdentityServices();

            return services;
        }

        private static void AddDbContextServices(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<TripmateDbContext>(options =>
            {
                options.UseSqlServer(connectionString);
            });

        }

        private static void AddIdentityServices(this IServiceCollection services)
        {
            services.AddIdentity<ApplicationUser, IdentityRole>()
               .AddEntityFrameworkStores<TripmateDbContext>()
               .AddDefaultTokenProviders();
        }

    }
}
