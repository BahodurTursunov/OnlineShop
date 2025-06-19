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
            await _next(context);

            context.Response.Body.Seek(0, SeekOrigin.Begin);
            var body = await new StreamReader(context.Response.Body).ReadToEndAsync();

            context.Response.Body.Seek(0, SeekOrigin.Begin);

            if (string.IsNullOrWhiteSpace(body) || context.Response.StatusCode != StatusCodes.Status200OK)
            {
                context.Response.Body = originalBodyStream;
                await responseBody.CopyToAsync(originalBodyStream);
                return;
            }

            object? parsedBody;
            try
            {
                parsedBody = JsonSerializer.Deserialize<object>(body);
            }
            catch
            {
                parsedBody = body;
            }

            var response = ApiResponse<object>.SuccessResponse(parsedBody);
            var wrappedBody = JsonSerializer.Serialize(response);

            context.Response.ContentType = "application/json";
            context.Response.ContentLength = System.Text.Encoding.UTF8.GetByteCount(wrappedBody);

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
