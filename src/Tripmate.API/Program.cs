
using Tripmate.API.Helper;

namespace Tripmate.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add All Services
            builder.Services.AddAllServices();

            var app = builder.Build();
            app.Run();
        }
    }
}
