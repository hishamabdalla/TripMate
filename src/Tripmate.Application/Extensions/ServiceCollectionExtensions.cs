using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tripmate.Application.Services.Abstractions.Identity;
using Tripmate.Application.Services.Identity;
using Tripmate.Application.Services.Identity.Login;
using Tripmate.Application.Services.Identity.Register;
using Tripmate.Application.Services.Identity.Token;
using Tripmate.Domain.AppSettings;
using Tripmate.Domain.Entities;
using Tripmate.Domain.Services.Interfaces.Identity;

namespace Tripmate.Application.Extension
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services,IConfiguration configuration)
        {
            // Register application services here
            services.RegisterApplicationServices(configuration);

            // Register options
           services.OptionsSetup( configuration);

            return services;
        }

        public static IServiceCollection RegisterApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<ILoginHandler, LoginHandler>();
            services.AddScoped<IRegisterHandler, RegisterHandler>();
            services.AddScoped<IAuthService, AuthService>();

            return services;
        }
        private static void OptionsSetup(this IServiceCollection services,IConfiguration configuration)
        {
            // Configure your application settings here

            services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));
        }

        
    }
}
