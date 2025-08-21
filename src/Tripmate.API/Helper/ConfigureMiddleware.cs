using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Tripmate.API.Middlewares;
using Tripmate.Infrastructure.Data.Context;
using Tripmate.Infrastructure.DbHelper.Seeding;

namespace Tripmate.API.Helper
{
    public static class ConfigureMiddleware
    {
        public static async Task ConfigureMiddlewareServices(this WebApplication app)
        {
            // Apply migrations at startup
            app.ApplyMigrations();

             await app.SeedData();

            // Configure the HTTP request pipeline.
           
                app.UseSwagger();
                app.UseSwaggerUI();

            // Custom exception middleware
            app.UseMiddleware<ExceptionMiddleware>();

            app.UseHttpsRedirection(); 
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
            // Enable CORS
            app.UseCors("AllowAllOrigins");
            app.UseStaticFiles();


        }
        private static void ApplyMigrations(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<TripmateDbContext>>();
            var context = scope.ServiceProvider.GetRequiredService<TripmateDbContext>();

            var pendingMigrations = context.Database.GetPendingMigrations();
            if (pendingMigrations.Any())
            {
                logger.LogInformation("Applying migrations...");
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

            using var scope=app.Services.CreateScope();

            var services = scope.ServiceProvider;

            var seeder = services.GetRequiredService<ISeeder>();

            await seeder.SeedAsync();

        }
    }
}
