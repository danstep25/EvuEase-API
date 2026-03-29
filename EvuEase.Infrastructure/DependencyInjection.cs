using Microsoft.Extensions.DependencyInjection;
using EvuEase.Application.Interfaces.Repositories;
using EvuEase.Infrastructure.Repositories;

namespace EvuEase.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructures(this IServiceCollection services)
        {
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ISystemLogRepository, SystemLogRepository>();
            services.AddScoped<IProgramRepository, ProgramRepository>();
            services.AddScoped<ISyTermRepository, SyTermRepository>();
            services.AddScoped<ICurriculaRepository, CurriculaRepository>();
            services.AddScoped<ICourseRepository, CourseRepository>();
            services.AddScoped<ITuitionFeeRepository, TuitionFeeRepository>();
            services.AddScoped<IOtherSchoolFeeRepository, OtherSchoolFeeRepository>();
            services.AddScoped<IMiscellaneousFeeRepository, MiscellaneousFeeRepository>();
            services.AddScoped<IStudentRepository, StudentRepository>();
    
            return services;
        }
    }
}

