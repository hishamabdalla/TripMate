using Serilog;
using Tripmate.Application.Extension;
using Tripmate.Infrastructure.Extensions;

namespace Tripmate.API.Helper
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAllServices(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddControllers();

            // Add Swagger service
            services.AddSwaggerService();
            // Add CORS policy
            services.AddCorsPolicy();
            // Add Application services
            services.AddApplicationServices(configuration);
            // Add Infrastructure services
            services.AddInfrastructureServices(configuration);

            return services;
        }

        private static void AddSwaggerService(this IServiceCollection services)
        {
            //Swagger configuration
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
        }

        private static IServiceCollection AddCorsPolicy(this IServiceCollection services)
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

        public static IHostBuilder AddSerilogService(this IHostBuilder host)
        {
            var config= new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(config)
                .Enrich.FromLogContext()
                .CreateLogger();

            

            return host.UseSerilog();

        }


    }
}
