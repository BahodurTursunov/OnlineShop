using BaseLibrary.Entities;
using BaseLibrary.Mapping;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.DependencyInjection;
using ServerLibrary.Authentication.Claim;
using ServerLibrary.Repositories.Contracts;
using ServerLibrary.Repositories.Implementations;
using ServerLibrary.Services.Contracts;
using ServerLibrary.Services.Contracts.Auth;
using ServerLibrary.Services.Contracts.Cache;
using ServerLibrary.Services.Contracts.Validation;
using ServerLibrary.Services.Implementations;
using ServerLibrary.Services.Implementations.Auth;
using ServerLibrary.Services.Implementations.Cache;
using ServerLibrary.Services.Implementations.Validation;
using ServerLibrary.SignalR;
using ServerLibrary.Validation;
using System.Threading.RateLimiting;

namespace ServerLibrary.DI
{
    public static class ServiceCollection
    {
        public static void AddMyServices(this IServiceCollection services)
        {
            /*   services.AddOpenTelemetry()
                   .WithTracing(tracerProviderBuilder =>
                   {
                       tracerProviderBuilder
                           .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("ProductService"))
                           .AddAspNetCoreInstrumentation()
                           .AddHttpClientInstrumentation()
                           .AddEntityFrameworkCoreInstrumentation()
                           .AddConsoleExporter();
                   })
                   .WithMetrics(metricsBuilder =>
                   {
                       metricsBuilder
                           .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("ProductService"))
                           .AddAspNetCoreInstrumentation()
                           .AddHttpClientInstrumentation()
                           .AddRuntimeInstrumentation()
                           .AddPrometheusExporter();
                   });*/
            services.AddScoped(typeof(ISqlRepository<>), typeof(SqlRepository<>));
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IJwtService, JwtService>();

            services.AddScoped<ICartRepository, CartRepository>();
            services.AddScoped<ICartService, CartService>();

            services.AddScoped<IRedisCacheService<Product>, RedisCacheService<Product>>();
            services.AddScoped(typeof(IRedisCacheService<>), typeof(RedisCacheService<>));


            services.AddScoped<ProductValidation>();
            services.AddScoped<CategoryValidation>();
            services.AddScoped<OrderValidation>();
            services.AddScoped<OrderItemValidation>();
            services.AddScoped<UserValidation>();
            services.AddScoped<IValidator<Product>, ProductValidation>();
            services.AddScoped<IValidator<Category>, CategoryValidation>();
            services.AddScoped<IValidator<User>, UserValidation>();
            services.AddScoped<IValidator<Order>, OrderValidation>();
            services.AddScoped<IValidator<OrderItem>, OrderItemValidation>();
            services.AddScoped(typeof(IEntityValidator<>), typeof(EntityValidator<>));

            services.AddCors(opt =>
            {
                opt.AddPolicy("AllowFrontend", builder =>
                {
                    builder.WithOrigins("https://")
                    .AllowAnyHeader()
                    .AllowAnyMethod();
                });
            });

            services.AddSignalR(hubOptions =>
            {
                hubOptions.EnableDetailedErrors = true;
                hubOptions.KeepAliveInterval = TimeSpan.FromMinutes(1);
            })
            .AddHubOptions<SupportHub>(options =>
            {
                options.EnableDetailedErrors = false;
                options.KeepAliveInterval = TimeSpan.FromMinutes(5);
            });

            services.AddRateLimiter(opt2 =>
            {
                opt2.AddFixedWindowLimiter("fixed", opt2 =>
                {
                    opt2.PermitLimit = 5;
                    opt2.Window = TimeSpan.FromSeconds(30);
                    opt2.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                    opt2.QueueLimit = 5;
                });
                opt2.OnRejected = async (context, cancellationToken) =>
                {
                    context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                    context.HttpContext.Response.Headers["Retry-After"] = "30";
                    await context.HttpContext.Response.WriteAsync("Rate limit exceeded. Please try again later.", cancellationToken);

                    var errorResponse = new
                    {
                        success = false,
                        message = "Rate limit exceeded. Please try again later.",
                        statusCode = 429
                    };
                    var message = System.Text.Json.JsonSerializer.Serialize(errorResponse);
                    await context.HttpContext.Response.WriteAsync(message, cancellationToken);
                };
            });

            services.AddMyClaims();
            services.AddAutoMapper(config => config.AddProfile<MappingProfile>(), typeof(MappingProfile));
        }
    }
}
