using Serilog;
using Tripmate.API.Helper;

namespace Tripmate.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add All Services
            builder.Services.AddAllServices(builder.Configuration);
            builder.Host.AddSerilogService();


            try
            {
                Log.Information("Application Starting Up");
                var app = builder.Build();
                await app.ConfigureMiddlewareServices();
                app.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application start-up failed");
                return;
            }
            finally
            {
                Log.CloseAndFlush();
            }




            }
    }
}
