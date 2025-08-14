
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Tripmate.Application.Services.Abstractions.Country;
using Tripmate.Application.Services.Abstractions.Identity;
using Tripmate.Application.Services.Countries;
using Tripmate.Application.Services.Identity;
using Tripmate.Application.Services.Identity.ForgotPassword;
using Tripmate.Application.Services.Identity.Login;
using Tripmate.Application.Services.Identity.RefreshTokens;
using Tripmate.Application.Services.Identity.Register;
using Tripmate.Application.Services.Identity.ResetPassword;
using Tripmate.Application.Services.Identity.Token;
using Tripmate.Application.Services.Identity.VerifyEmail;
using Tripmate.Domain.AppSettings;
using Tripmate.Domain.Common.Response;
using Tripmate.Domain.Services.Interfaces.Identity;


namespace Tripmate.Application.Extension
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Register application services here
            services.RegisterApplicationServices(configuration);


            // Register options
            services.OptionsSetup(configuration);

            // Register AutoMapper
            services.AddAutoMapperServices();

            services.AddValiadiationErrorHandlingServices();

            return services;
        }

        public static IServiceCollection RegisterApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IRefreshTokenHandler, RefreshTokenHandler>();
            services.AddScoped<ILoginHandler, LoginHandler>();
            services.AddScoped<IRegisterHandler, RegisterHandler>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IRefreshTokenHandler, RefreshTokenHandler>();
            services.AddScoped<IEmailHandler, EmailHandler>();
            services.AddScoped<IForgetPasswordHandler, ForgetPasswordHandler>();
            services.AddScoped<IResetPasswordHandler, ResetPasswordHandler>();
            services.AddScoped<ICountryService, CountryService>();
            services.AddMemoryCache();
            return services;
        }
        private static void OptionsSetup(this IServiceCollection services, IConfiguration configuration)
        {
            // Configure your application settings here
            services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));


        }

        private static void AddAutoMapperServices(this IServiceCollection services)
        {
            var applicationsAssembly = Assembly.GetExecutingAssembly();
            services.AddAutoMapper(applicationsAssembly);
        }
        private static void AddValiadiationErrorHandlingServices(this IServiceCollection services)
        {
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = actionContext =>
                {
                    var errors = actionContext.ModelState
                        .Where(e => e.Value.Errors.Count > 0)
                        .SelectMany(x => x.Value.Errors)
                        .Select(x => x.ErrorMessage).ToList();

                    var result = new ApiResponse<string>(
                        success: false,
                        statusCode: 400,
                        message: "Validation errors",
                        errors: errors
                        );


                    return new BadRequestObjectResult(result);
                };
            }
            );


        }
    }
}