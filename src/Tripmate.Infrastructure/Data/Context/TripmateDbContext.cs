using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tripmate.Domain.Entities;

namespace Tripmate.Infrastructure.Data.Context
{
    public class TripmateDbContext(DbContextOptions<TripmateDbContext> options) :IdentityDbContext<ApplicationUser>(options)
    {
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }


    }
}
