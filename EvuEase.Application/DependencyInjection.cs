using Microsoft.Extensions.DependencyInjection;
using EvuEase.Application.Interfaces.Services;
using EvuEase.Application.Services;
using EvuEase.Application.Mappings;

namespace EvuEase.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        // AutoMapper
        services.AddAutoMapper(typeof(UserMappingProfile), typeof(SystemLogMappingProfile));

        //Services
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ISystemLogService, SystemLogService>();
        services.AddScoped<ILookupService, LookupService>();

        return services;
    }
}

