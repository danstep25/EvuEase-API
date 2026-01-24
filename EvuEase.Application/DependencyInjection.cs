using Microsoft.Extensions.DependencyInjection;
using EvuEase.Application.Interfaces.Services;
using EvuEase.Application.Services;

namespace EvuEase.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        //Services
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IAuthService, AuthService>();

        return services;
    }
}

