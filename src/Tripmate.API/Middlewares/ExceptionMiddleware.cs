using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using Tripmate.Domain.Common.Response;
using Tripmate.Domain.Exceptions;

namespace Tripmate.API.Middlewares
{
    public class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (RedisConnectionException redisException)
            {
                logger.LogError(redisException, "Redis connection failed at {Path}. Application should continue with fallback cache",
                    context.Request.Path);
                await HandleExceptionAsync(context, StatusCodes.Status503ServiceUnavailable,
                    "Redis connection failed");
            }
            catch (RedisException redisException)
            {
                logger.LogError(redisException, "Redis operation failed at {Path}. Fallback mechanisms should handle this",
                    context.Request.Path);
                await HandleExceptionAsync(context, StatusCodes.Status500InternalServerError,
                    "A caching error occurred, but the request should still be processed");
            }
            // Catch any unhandled exceptions and handle them
            catch (NotFoundException notFoundException)
            {
                logger.LogWarning("NotFound Exception: {Message} at {Path} for User {User}", 
                    notFoundException.Message, context.Request.Path, context.User?.Identity?.Name ?? "Anonymous");
                await HandleExceptionAsync(context, StatusCodes.Status404NotFound, notFoundException.Message);
            }
            catch(ImageValidationException imageValidationException)
            {
                logger.LogWarning("Image Validation Exception: {Message} at {Path}", 
                    imageValidationException.Message, context.Request.Path);
                await HandleExceptionAsync(context, StatusCodes.Status400BadRequest, imageValidationException.Message);
            }
            catch (BadRequestException badRequestException)
            {
                logger.LogWarning("Bad Request Exception: {Message} at {Path} for User {User}", 
                    badRequestException.Message, context.Request.Path, context.User?.Identity?.Name ?? "Anonymous");
                await HandleExceptionAsync(context, StatusCodes.Status400BadRequest, badRequestException.Message);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unhandled exception occurred at {Path} for User {User}. Method: {Method}", 
                    context.Request.Path, context.User?.Identity?.Name ?? "Anonymous", context.Request.Method);
                await HandleExceptionAsync(context, StatusCodes.Status500InternalServerError, "An unexpected error occurred. Please try again later.");
            }
        }
        
        private static async Task HandleExceptionAsync(HttpContext context, int statusCode, string message)
        {
            context.Response.Clear();
            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";
            
            var response = new ApiResponse<string>(
                success: false,
                statusCode: statusCode,
                message: message
            );

            await context.Response.WriteAsJsonAsync(response);
        }
    }
}
