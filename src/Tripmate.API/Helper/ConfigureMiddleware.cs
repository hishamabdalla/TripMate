using Microsoft.EntityFrameworkCore;
using Tripmate.API.Middlewares;
using Tripmate.Infrastructure.Data.Context;

namespace Tripmate.API.Helper
{
    public static class ConfigureMiddleware
    {
        public static void ConfigureMiddlewareServices(this WebApplication app)
        {
            // Apply migrations at startup
            app.ApplyMigrations();

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
    }
}
