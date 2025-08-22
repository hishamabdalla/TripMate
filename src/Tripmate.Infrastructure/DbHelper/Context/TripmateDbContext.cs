using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Tripmate.Domain.Entities.Models;
using Tripmate.Infrastructure.Data.Configuration;

namespace Tripmate.Infrastructure.Data.Context
{
    public class TripmateDbContext(DbContextOptions<TripmateDbContext> options) :IdentityDbContext<ApplicationUser>(options)
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
          

            modelBuilder.ApplySoftDeleteQueryFilter();
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        }
        public DbSet<ApplicationUser> applicationUsers { get; set; }
        public DbSet<Attraction> Attractions { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<Hotel> Hotels { get; set; }
        public DbSet<Region> Regions { get; set; }
        public DbSet<Restaurant> Restaurants { get; set; }
        public DbSet<Review> Reviews { get; set; }
    }
}
