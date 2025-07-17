using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ServerLibrary.Middleware
{
    public class TooManyRequestMiddleware
    {
        private readonly RequestDelegate _next;

        public TooManyRequestMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            await _next(context);

            if (context.Response.StatusCode == StatusCodes.Status429TooManyRequests)
            {
                var logger = context.RequestServices.GetRequiredService<ILogger<TooManyRequestMiddleware>>();
                logger.LogWarning("Request blocked with 429 Too Many Requests for {Path}", context.Request.Path);
            }
        }
    }
}
