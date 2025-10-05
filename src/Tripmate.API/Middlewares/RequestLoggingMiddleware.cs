using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace Tripmate.API.Middlewares
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();
            var requestId = Guid.NewGuid().ToString("N")[..8];
            
            // Log incoming request
            _logger.LogInformation("Request {RequestId} started: {Method} {Path} from {RemoteIpAddress} User: {User}", 
                requestId,
                context.Request.Method, 
                context.Request.Path + context.Request.QueryString,
                context.Connection.RemoteIpAddress?.ToString() ?? "Unknown",
                context.User?.Identity?.Name ?? "Anonymous");

            // Add request ID to response headers for tracking
            context.Response.Headers["X-Request-ID"] = requestId;

            try
            {
                await _next(context);
            }
            finally
            {
                stopwatch.Stop();
                
                // Log response
                var logLevel = context.Response.StatusCode >= 400 ? LogLevel.Warning : LogLevel.Information;
                
                _logger.Log(logLevel, "Request {RequestId} completed: {Method} {Path} responded {StatusCode} in {ElapsedMilliseconds}ms User: {User}",
                    requestId,
                    context.Request.Method,
                    context.Request.Path + context.Request.QueryString,
                    context.Response.StatusCode,
                    stopwatch.ElapsedMilliseconds,
                    context.User?.Identity?.Name ?? "Anonymous");
            }
        }
    }
}