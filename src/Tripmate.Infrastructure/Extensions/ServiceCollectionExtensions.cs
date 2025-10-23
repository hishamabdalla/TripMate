using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Tripmate.Domain.AppSettings;
using Tripmate.Domain.Common.Response;
using Tripmate.Domain.Entities.Models;
using Tripmate.Domain.Enums;
using Tripmate.Domain.Interfaces;
using Tripmate.Infrastructure.Data.Context;
using Tripmate.Infrastructure.DbHelper.Seeding;
using Tripmate.Infrastructure.Repositories;


namespace Tripmate.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services,IConfiguration configuration)
        {

            // Add DbContext services
            services.AddDbContextServices(configuration);

            //register IUnitOfWork
            services.AddScoped<IUnitOfWork,UnitOfWork>();

            // Add Identity services
            services.AddIdentityServices();

            // Add Authentication services
            services.AddAuthenticationServices(configuration);

            // Add Authorization services
            services.AddAuthorizationServices();

            services.AddScoped<ISeeder, Seeder>();

            services.Configure<AppUsersSettings>(configuration.GetSection("AppUsersSettings"));

            return services;
        }

        private static void AddDbContextServices(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("cs");
            services.AddDbContext<TripmateDbContext>(options =>
            {
                options.UseSqlServer(connectionString);
            });

        }

        private static void AddIdentityServices(this IServiceCollection services)
        {
            services.AddIdentity<ApplicationUser, IdentityRole>(options=>
            {
                options.Tokens.PasswordResetTokenProvider=TokenOptions.DefaultEmailProvider;

                options.Password.RequiredLength = 6;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireDigit = true;
                options.Password.RequireNonAlphanumeric = false;

            })
               .AddEntityFrameworkStores<TripmateDbContext>()
               .AddDefaultTokenProviders();
        }

        private static void AddAuthenticationServices(this IServiceCollection services , IConfiguration configuration)
        {
            
            // Configure JWT authentication
            var jwtSection = configuration.GetSection("JwtSettings");
            services.Configure<JwtSettings>(jwtSection);

            var jwtSettings = jwtSection.Get<JwtSettings>();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                
                options.Events = new JwtBearerEvents
                {
                    OnChallenge = async context =>
                    {
                        context.HandleResponse();
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        context.Response.ContentType = "application/json";

                        var response = new ApiResponse<string>(false, StatusCodes.Status401Unauthorized, "Unauthorized! Please log in.", null);

                        await context.Response.WriteAsJsonAsync(response);
                    },
                    OnForbidden = async context =>
                    {
                        context.Response.StatusCode = StatusCodes.Status403Forbidden;
                        context.Response.ContentType = "application/json";

                        var response = new ApiResponse<string>(
                            false,
                            StatusCodes.Status403Forbidden,
                            "Access denied! You don’t have permission to perform this action.",
                            null
                        );

                        await context.Response.WriteAsJsonAsync(response);
                    }
                };
                 options.TokenValidationParameters = new TokenValidationParameters
                     {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret)),
                    ClockSkew = TimeSpan.Zero // Optional: Set clock skew to zero for immediate expiration
                };
            });
        }

        private static void AddAuthorizationServices(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                // Role-based policies
                options.AddPolicy("AdminOnly", policy => policy.RequireRole(UserRoles.Admin));
                options.AddPolicy("UserOnly", policy => policy.RequireRole(UserRoles.User));
                options.AddPolicy("AdminOrUser", policy => policy.RequireRole(UserRoles.Admin, UserRoles.User));

             
            });
        }


    }
}
