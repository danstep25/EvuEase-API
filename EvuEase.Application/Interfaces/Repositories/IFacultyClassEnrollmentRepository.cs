using EvuEase.Application.DTOs.ClassRoster;
using EvuEase.Application.DTOs.Student;
using EvuEase.Domain.Entities;

namespace EvuEase.Application.Interfaces.Repositories;

public interface IFacultyClassEnrollmentRepository
{
    Task<IReadOnlyList<ClassRosterStudentResponse>> GetStudentRowsForClassAsync(long facultyClassId, CancellationToken cancellationToken = default);

    Task ReplaceAllForClassAsync(long facultyClassId, IReadOnlyList<FacultyClassEnrollment> enrollments, CancellationToken cancellationToken = default);

    
    Task<ClassRosterStudentResponse?> UpdateOfficialGradeAsync(
        long enrollmentId,
        string? officialGrade,
        string? storedRemarks,
        CancellationToken cancellationToken = default);

    
    Task<IReadOnlyList<StudentClassEnrollmentRowDto>> GetEnrollmentRowsForStudentAsync(
        long studentId,
        CancellationToken cancellationToken = default);

    Task<int> CountEnrollmentsForClassAsync(long facultyClassId, CancellationToken cancellationToken = default);

    
    Task<ClassRosterStudentResponse?> AddEnrollmentAsync(
        long facultyClassId,
        long studentId,
        CancellationToken cancellationToken = default);

    Task<bool> StudentHasCourseEnrollmentAsync(
        long studentId,
        string courseCode,
        CancellationToken cancellationToken = default);

    Task<bool> RemoveEnrollmentAsync(long facultyClassId, long enrollmentId, CancellationToken cancellationToken = default);
}
