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
using ServerLibrary.Services;
using ServerLibrary.Services.Contracts;
using ServerLibrary.Services.Contracts.Auth;
using ServerLibrary.Services.Implementations;
using ServerLibrary.Services.Implementations.Auth;
using ServerLibrary.SignalR;
using ServerLibrary.Validation;
using System.Threading.RateLimiting;

namespace ServerLibrary.DI
{
    public static class ServiceCollection
    {
        public static void AddMyServices(this IServiceCollection services)
        {
            services.AddScoped(typeof(ISqlRepository<>), typeof(SqlRepository<>));
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IJwtService, JwtService>();

            services.AddScoped<ICartRepository, CartRepository>();
            services.AddScoped<ICartService, CartService>();

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

            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = "localhost"; // Adjust as needed for your Redis server
                options.InstanceName = "OnlineShopCache"; // Optional prefix for cache keys
            });

            services.AddRateLimiter(opt =>
            {
                opt.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpcontext =>
                RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: httpcontext.User.Identity?.Name ?? httpcontext.Request.Headers.Host.ToString(),
                    factory: partition => new FixedWindowRateLimiterOptions
                    {
                        AutoReplenishment = true,
                        PermitLimit = 30,
                        QueueLimit = 30,
                        Window = TimeSpan.FromMinutes(1)
                    }));
            });

            services.AddRateLimiter(opt2 =>
            {
                opt2.AddFixedWindowLimiter("fixed", opt2 =>
                {
                    opt2.PermitLimit = 5;
                    opt2.Window = TimeSpan.FromSeconds(20);
                    opt2.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                    opt2.QueueLimit = 5;
                });
            });


            services.AddMyClaims();
            services.AddAutoMapper(typeof(UserProfile));
        }
    }
}
