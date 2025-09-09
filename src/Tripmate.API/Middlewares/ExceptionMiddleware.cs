using Tripmate.Domain.Common.Response;
using Tripmate.Domain.Exceptions;

namespace Tripmate.API.Middlewares
{
    public class ExceptionMiddleware(RequestDelegate next)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next(context);
            }
            // Catch any unhandled exceptions and handle them
            catch (NotFoundException notFoundException)
            {
                await HandleExceptionAsync(context, StatusCodes.Status404NotFound, notFoundException.Message);
            }
            catch(ImageValidationException imageValidationException)
            {
                await HandleExceptionAsync(context, StatusCodes.Status400BadRequest, imageValidationException.Message);
            }
            

            catch (BadRequestException badRequestException)
            {
                await HandleExceptionAsync(context, StatusCodes.Status400BadRequest, badRequestException.Message);
            }
          
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, StatusCodes.Status500InternalServerError,ex.Message);
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
