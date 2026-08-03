using EvuEase.Domain.Entities;

namespace EvuEase.Application.Interfaces.Services;

public interface IStudentCurriculumAssignmentService
{
    Task TryAssignDefaultCurriculumForFirstYearAsync(
        Student student,
        string reason,
        CancellationToken cancellationToken = default);

    Task AssignCurriculumIfMissingAsync(
        Student student,
        string curriculumCode,
        string reason,
        CancellationToken cancellationToken = default);
}
