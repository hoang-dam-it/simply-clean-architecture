using Microsoft.Extensions.DependencyInjection;

namespace Shop.Persistence
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPersistence(this IServiceCollection services)
        {
            return services;
        }
    }
}
