using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tripmate.Domain.AppSettings;
using Tripmate.Domain.Entities.Models;

namespace Tripmate.Infrastructure.DbHelper.Seeding.DataSeeder
{
    public class IdentitySeeder
    {
        // Implementation for IdentitySeeder goes here

        public async static Task SeedRolesAndAdminAsync(IServiceProvider serviceProvider, IOptions<AppUsersSettings> options)
        {
            // Implementation for seeding roles and admin user goes here
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            var roles = new[] { "Admin", "User" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }

            }

            var users = options.Value;
            foreach (var user in users.AppUsers)
            {
                if (user != null)
                {
                    var existUser = await userManager.FindByEmailAsync(user.Email);
                    if (existUser == null)
                    {
                        var applicationUser = new ApplicationUser();
                        applicationUser.Email = user.Email;
                        applicationUser.UserName = user.UserName;
                        applicationUser.EmailConfirmed = true;
                        applicationUser.IsActive = true;
                        applicationUser.Country = user.Country;

                        var result = await userManager.CreateAsync(applicationUser, user.Password);
                        if (result.Succeeded)
                        {
                            await userManager.AddToRoleAsync(applicationUser, user.Role);
                        }
                    }

                }
            }
        }
    }
}
