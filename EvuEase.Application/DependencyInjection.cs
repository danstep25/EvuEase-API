using Microsoft.Extensions.DependencyInjection;
using EvuEase.Application.Interfaces.Services;
using EvuEase.Application.Services;
using EvuEase.Application.Mappings;

namespace EvuEase.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(UserMappingProfile), typeof(SystemLogMappingProfile), typeof(ProgramMappingProfile), typeof(SyTermMappingProfile), typeof(CurriculaMappingProfile), typeof(StudentMappingProfile));
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ISystemLogService, SystemLogService>();
        services.AddScoped<ILookupService, LookupService>();
        services.AddScoped<IProgramService, ProgramService>();
        services.AddScoped<ISyTermService, SyTermService>();
        services.AddScoped<ICurriculaService, CurriculaService>();
        services.AddScoped<ICourseService, CourseService>();
        services.AddScoped<ITuitionFeeService, TuitionFeeService>();
        services.AddScoped<IOtherSchoolFeeService, OtherSchoolFeeService>();
        services.AddScoped<IMiscellaneousFeeService, MiscellaneousFeeService>();
        services.AddScoped<IStudentService, StudentService>();

        return services;
    }
}

