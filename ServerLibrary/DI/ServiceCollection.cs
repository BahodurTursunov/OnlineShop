using Microsoft.Extensions.DependencyInjection;
using ServerLibrary.Repositories.Contracts;
using ServerLibrary.Repositories.Implementations;

namespace ServerLibrary.DI
{
    public static class ServiceCollection
    {
        public static void AddServices(this IServiceCollection services)
        {
            services.AddScoped(typeof(ISqlRepository<>), typeof(SqlRepository<>));

        }
    }
}
