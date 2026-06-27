using Microsoft.Extensions.DependencyInjection;
using EvuEase.Application.Interfaces.Persistence;
using EvuEase.Application.Interfaces.Repositories;
using EvuEase.Application.Interfaces.Services;
using EvuEase.Infrastructure.Persistence;
using EvuEase.Infrastructure.Repositories;
using EvuEase.Infrastructure.Storage;

namespace EvuEase.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructures(this IServiceCollection services)
        {
            services.AddScoped<IApplicationUnitOfWork, EfApplicationUnitOfWork>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ISystemLogRepository, SystemLogRepository>();
            services.AddScoped<IProgramRepository, ProgramRepository>();
            services.AddScoped<ISyTermRepository, SyTermRepository>();
            services.AddScoped<ICurriculaRepository, CurriculaRepository>();
            services.AddScoped<ICourseRepository, CourseRepository>();
            services.AddScoped<ITuitionFeeRepository, TuitionFeeRepository>();
            services.AddScoped<IOtherSchoolFeeRepository, OtherSchoolFeeRepository>();
            services.AddScoped<IMiscellaneousFeeRepository, MiscellaneousFeeRepository>();
            services.AddScoped<IDownpaymentRepository, DownpaymentRepository>();
            services.AddScoped<IPaymentSchemeRepository, PaymentSchemeRepository>();
            services.AddScoped<IStudentRepository, StudentRepository>();
            services.AddScoped<IStudentPortalPasswordResetRepository, StudentPortalPasswordResetRepository>();
            services.AddScoped<IStudentCurriculumHistoryRepository, StudentCurriculumHistoryRepository>();
            services.AddScoped<IGradingSchemeBasisRepository, GradingSchemeBasisRepository>();
            services.AddScoped<IGradeScaleRowRepository, GradeScaleRowRepository>();
            services.AddScoped<IFacultyClassRepository, FacultyClassRepository>();
            services.AddScoped<IFacultyClassEnrollmentRepository, FacultyClassEnrollmentRepository>();
            services.AddScoped<ICreditRequestRepository, CreditRequestRepository>();
            services.AddScoped<ICreditRequestLineRepository, CreditRequestLineRepository>();
            services.AddScoped<ICreditRequestSignedDocumentStore, CreditRequestSignedDocumentStore>();
            services.AddScoped<IArchiveRepository, ArchiveRepository>();
            services.AddScoped<IAnalyticsRepository, AnalyticsRepository>();

            return services;
        }
    }
}

