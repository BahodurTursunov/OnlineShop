using BaseLibrary.Mapping;
using Microsoft.Extensions.DependencyInjection;
using ServerLibrary.Authentication.Claim;
using ServerLibrary.Repositories.Contracts;
using ServerLibrary.Repositories.Implementations;
using ServerLibrary.Services;
using ServerLibrary.Services.Contracts;
using ServerLibrary.Services.Contracts.Auth;
using ServerLibrary.Services.Implementations;
using ServerLibrary.Services.Implementations.Auth;

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



            services.AddMyClaims();
            services.AddAutoMapper(typeof(UserProfile));
        }
    }
}
