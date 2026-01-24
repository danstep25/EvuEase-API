using Microsoft.Extensions.DependencyInjection;
using EvuEase.Application.Interfaces.Repositories;
using EvuEase.Infrastructure.Repositories;

namespace EvuEase.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructures(this IServiceCollection services)
        {
            //Repository
            services.AddScoped<IUserRepository, UserRepository>();
    
            return services;
        }
    }
}

