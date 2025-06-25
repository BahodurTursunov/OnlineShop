using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ServerLibrary.Exceptions;
using System.Net;
using System.Text.Json;

namespace ServerLibrary.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogError(ex, ex.Message);
                context.Response.StatusCode = (int)HttpStatusCode.NotFound; // 404
                var response = new { message = ex.Message };
                await context.Response.WriteAsync(JsonSerializer.Serialize(response));
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, ex.Message);
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest; // 400
                var response = new { message = ex.Message };
                await context.Response.WriteAsync(JsonSerializer.Serialize(response));
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError(ex, ex.Message);
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized; // 401
                var response = new { message = "Your password is incorrect!" };
                await context.Response.WriteAsync(JsonSerializer.Serialize(response));
            }
            catch (UsernameAlreadyExistsException ex)
            {
                _logger.LogError(ex, ex.Message);
                context.Response.StatusCode = (int)HttpStatusCode.Conflict; // 409
                var response = new { message = "Username is already exist" };
                await context.Response.WriteAsync(JsonSerializer.Serialize(response));
            }
            catch (UserMailAlreadyExistsException ex)
            {
                _logger.LogError(ex, ex.Message);
                context.Response.StatusCode = (int)HttpStatusCode.Conflict; // 409
                var response = new { message = "Email already exist" };
                await context.Response.WriteAsync(JsonSerializer.Serialize(response));
            }
            catch (DbUpdateDeleteException ex)
            {
                _logger.LogError(ex, ex.Message);
                context.Response.StatusCode = (int)HttpStatusCode.Conflict;
                var response = new { message = "Cannot delete or update object" };
                await context.Response.WriteAsync(JsonSerializer.Serialize(response));
            }
            catch (ValidationException validationEx)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;

                var messages = validationEx.Errors
                    .Select(e => e.ErrorMessage)
                    .ToArray();

                //var result = new ApiResponse<string>(
                //    success: false,
                //    message: string.Join("; ", messages),
                //    data: null
                //);

                var json = JsonSerializer.Serialize(messages);
                await context.Response.WriteAsync(json);
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception");
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError; // 500
                var response = new { message = "Internal server error", details = ex.Message };
                await context.Response.WriteAsync(JsonSerializer.Serialize(response));
            }
        }
    }
}
