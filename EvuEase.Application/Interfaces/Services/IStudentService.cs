using EvuEase.Application.Common;
using EvuEase.Application.DTOs.Student;

namespace EvuEase.Application.Interfaces.Services;

public interface IStudentService
{
    Task<PagedResults<StudentResponse>> GetAllStudents(StudentRequest request);
    Task<StudentResponse?> GetStudentByIdAsync(long id);
    Task<StudentResponse> CreateStudentAsync(CreateStudentRequest request);
    Task<StudentResponse> UpdateStudentAsync(UpdateStudentRequest request);
    Task DeleteStudentAsync(long id);

    
    Task<StudentEnrollmentOverviewResponse?> GetStudentEnrollmentOverviewAsync(long id, CancellationToken cancellationToken = default);

    Task<StudentResponse?> MigrateStudentCurriculumAsync(
        long id,
        MigrateStudentCurriculumRequest request,
        string? migratedBy,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<StudentCurriculumHistoryResponse>?> GetStudentCurriculumHistoryAsync(
        long id,
        CancellationToken cancellationToken = default);
}
