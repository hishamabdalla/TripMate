using Tripmate.Application.Extension;
using Tripmate.Infrastructure.Extensions;

namespace Tripmate.API.Helper
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAllServices(this IServiceCollection services,IConfiguration configuration)
        {

            services.AddControllers();
            // Add Swagger service
            services.AddSwaggerService();
            // Add CORS policy
            services.AddCorsPolicy();

            // Add Infrastructure services
            services.AddInfrastructureServices(configuration);
            // Add Application services
            services.AddApplicationServices(configuration);
            return services;
        }

        private static void AddSwaggerService(this IServiceCollection services)
        {
            //Swagger configuration
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
        }

        public static IServiceCollection AddCorsPolicy(this IServiceCollection services)
        {
            // Add CORS policy
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigins",
                    builder =>
                    {
                        builder.AllowAnyOrigin()
                               .AllowAnyMethod()
                               .AllowAnyHeader();
                    });
            });
            return services;
        }


    }
}
