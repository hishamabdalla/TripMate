using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tripmate.API.Middlewares;
using Tripmate.Domain.AppSettings;
using Tripmate.Infrastructure.Data.Context;
using Tripmate.Infrastructure.DbHelper.Seeding;
using Tripmate.Infrastructure.DbHelper.Seeding.DataSeeder;

namespace Tripmate.API.Helper
{
    public static class ConfigureMiddleware
    {
        public static async Task ConfigureMiddlewareServices(this WebApplication app)
        {
            var logger = app.Services.GetRequiredService<ILogger<Program>>();
            logger.LogInformation("Starting middleware configuration");

            // Apply migrations at startup
            app.ApplyMigrations();

            await app.SeedData();

            // Configure the HTTP request pipeline.
            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();

            // Add request logging middleware
            app.UseMiddleware<RequestLoggingMiddleware>();

            // Custom exception middleware
            app.UseMiddleware<ExceptionMiddleware>();

            
            app.MapControllers();
            // Enable CORS
            app.UseCors("AllowAllOrigins");
            app.UseStaticFiles();

            logger.LogInformation("Middleware configuration completed");
        }
        
        private static void ApplyMigrations(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<TripmateDbContext>>();
            var context = scope.ServiceProvider.GetRequiredService<TripmateDbContext>();

            var pendingMigrations = context.Database.GetPendingMigrations();
            if (pendingMigrations.Any())
            {
                logger.LogInformation("Applying {Count} pending migrations: {Migrations}", 
                    pendingMigrations.Count(), string.Join(", ", pendingMigrations));
                context.Database.Migrate();
                logger.LogInformation("Migrations applied successfully.");
            }
            else
            {
                logger.LogInformation("No pending migrations found.");
            }
        }

        private static async Task SeedData(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
            var services = scope.ServiceProvider;

            var options = services.GetRequiredService<IOptions<AppUsersSettings>>();

            try
            {
                logger.LogInformation("Starting data seeding process");
                var seeder = services.GetRequiredService<ISeeder>();
                await seeder.SeedAsync();
                logger.LogInformation("Data seeding completed successfully");
                await IdentitySeeder.SeedRolesAndAdminAsync(services, options);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Data seeding failed");
                throw;
            }
        }
    }
}
