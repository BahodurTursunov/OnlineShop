using BaseLibrary.Mapping;
using Microsoft.Extensions.DependencyInjection;
using ServerLibrary.Repositories.Contracts;
using ServerLibrary.Repositories.Implementations;
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
            /*  services.AddScoped(typeof(ISqlRepository<>), typeof(SqlRepository<>));
              services.AddScoped<IUserService, UserService>();
              services.AddScoped<IAuthService, AuthService>();*/
            //
            services.AddScoped(typeof(ISqlRepository<>), typeof(SqlRepository<>));
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IJwtService, JwtService>();

            services.AddAutoMapper(typeof(UserProfile));
        }
    }
}
