using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Text.Json.Serialization;
using Tripmate.Application.Extension;
using Tripmate.Infrastructure.Extensions;

namespace Tripmate.API.Helper
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAllServices(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddControllers(options =>
            {
                //Do not Make non-nullable reference types required by default
                options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
            }).AddJsonOptions(options =>
            {
                 //When User add string value instead of integer value for enum properties in json it will be handled properly
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });


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
            services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\""
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        },

                        Name = "Bearer",
                        In = ParameterLocation.Header
                    },

                     new List<string>()
                }
                 });
        });


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

        private static IServiceCollection AddCustomAuthorizationHandlers(this IServiceCollection services)
        {
            // Register HTTP context accessor for authorization handlers
            services.AddHttpContextAccessor();
           
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
