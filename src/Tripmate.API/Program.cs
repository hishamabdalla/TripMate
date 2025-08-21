
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Tripmate.API.Helper;
using Tripmate.Application.Services.Identity.VerifyEmail;
using Tripmate.Infrastructure.Data.Context;

namespace Tripmate.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add All Services
            builder.Services.AddAllServices(builder.Configuration);

            var app = builder.Build();
            // Configure Middleware
            await app.ConfigureMiddlewareServices();

            app.Run();
        }
    }
}
