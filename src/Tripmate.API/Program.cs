
using Microsoft.EntityFrameworkCore;
using Tripmate.API.Helper;
using Tripmate.Application.Services.Identity.VerifyEmail;
using Tripmate.Infrastructure.Data.Context;

namespace Tripmate.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add All Services
            builder.Services.AddAllServices(builder.Configuration);

            var app = builder.Build();
            // Configure Middleware
            app.ConfigureMiddlewareServices();

            app.Run();
        }
    }
}
