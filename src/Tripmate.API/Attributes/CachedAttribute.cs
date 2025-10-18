using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text;
using System.Text.Json;
using Tripmate.Application.Services.Caching;

namespace Tripmate.API.Attributes
{
    public class CachedAttribute:Attribute,IAsyncActionFilter
    {
        private readonly int _expirationInHour;
        public CachedAttribute(int expirationInHour)
        {
            _expirationInHour = expirationInHour;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
           var cacheService=  context.HttpContext.RequestServices.GetRequiredService<ICacheService>();
             var cacheKey = GenerateCacheKeyFromRequest(context.HttpContext.Request);

             var cachedResponse = await cacheService.GetAsync<object>(cacheKey);

            if(!string.IsNullOrEmpty(cachedResponse?.ToString()))
            {
                var contentResult = new ContentResult
                {
                    Content = JsonSerializer.Serialize(cachedResponse),
                    ContentType = "application/json",
                    StatusCode = 200
                };
                context.Result = contentResult;
                return;
            }
            var executedContext = await next();
            if (executedContext.Result is OkObjectResult okObjectResult)
            {
                await cacheService.SetAsync(cacheKey, okObjectResult.Value, TimeSpan.FromHours(_expirationInHour));
            }

        }

        private string GenerateCacheKeyFromRequest(HttpRequest request)
        {
            var keyBuilder = new StringBuilder();
            keyBuilder.Append($"{request.Path}");
            foreach (var (key, value) in request.Query.OrderBy(x => x.Key))
            {
                keyBuilder.Append($"|{key}-{value}");
            }
            return keyBuilder.ToString();
        }

    }
}
