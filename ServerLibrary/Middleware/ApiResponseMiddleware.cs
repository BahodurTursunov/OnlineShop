using BaseLibrary.DTOs;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

public class ApiResponseMiddleware
{
    private readonly RequestDelegate _next;

    public ApiResponseMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var originalBodyStream = context.Response.Body;

        using var responseBody = new MemoryStream();
        context.Response.Body = responseBody;

        try
        {
            await _next(context); // вызываем следующий middleware

            context.Response.Body.Seek(0, SeekOrigin.Begin);
            var body = await new StreamReader(context.Response.Body).ReadToEndAsync();

            context.Response.Body.Seek(0, SeekOrigin.Begin);

            if (string.IsNullOrWhiteSpace(body))
            {
                context.Response.Body = originalBodyStream;
                return;
            }

            var response = ApiResponse<object>.SuccessResponse(JsonSerializer.Deserialize<object>(body));
            var wrappedBody = JsonSerializer.Serialize(response);

            context.Response.ContentType = "application/json";
            context.Response.ContentLength = wrappedBody.Length;

            context.Response.Body = originalBodyStream;
            await context.Response.WriteAsync(wrappedBody);
        }
        catch (Exception ex)
        {
            context.Response.Body = originalBodyStream;
            context.Response.StatusCode = 500;

            var errorResponse = ApiResponse<string>.FailureResponse("Internal Server Error: " + ex.Message);
            var errorJson = JsonSerializer.Serialize(errorResponse);

            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(errorJson);
        }
    }
}
