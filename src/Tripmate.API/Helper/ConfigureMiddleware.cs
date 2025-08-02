namespace Tripmate.API.Helper
{
    public static class ConfigureMiddleware
    {
        public static void ConfigureMiddlewareServices(this WebApplication app)
        {
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseHttpsRedirection(); 
            app.UseAuthorization();
            app.MapControllers();
            // Enable CORS
            app.UseCors("AllowAllOrigins");

        }
    }
}
