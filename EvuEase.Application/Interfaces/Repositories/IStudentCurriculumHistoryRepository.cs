using EvuEase.Domain.Entities;

namespace EvuEase.Application.Interfaces.Repositories;

public interface IStudentCurriculumHistoryRepository
{
    Task<StudentCurriculumHistory> CreateAsync(StudentCurriculumHistory history, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<StudentCurriculumHistory>> GetByStudentIdAsync(
        long studentId,
        CancellationToken cancellationToken = default);
}
