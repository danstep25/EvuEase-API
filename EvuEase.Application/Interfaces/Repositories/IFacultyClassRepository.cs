using EvuEase.Domain.Entities;

namespace EvuEase.Application.Interfaces.Repositories;

public interface IFacultyClassRepository
{
    Task<IReadOnlyList<FacultyClass>> GetAllAsync(string? search, CancellationToken cancellationToken = default);

    
    
    
    
    Task<IReadOnlyList<FacultyClass>> GetForGradeRosterLookupAsync(
        IReadOnlyList<string> academicTermMatchKeys,
        string? search,
        CancellationToken cancellationToken = default);

    Task<FacultyClass?> GetByIdAsync(long id, CancellationToken cancellationToken = default);

    Task<FacultyClass> CreateAsync(FacultyClass entity, CancellationToken cancellationToken = default);

    Task SoftDeleteFacultyClassAsync(FacultyClass entity, CancellationToken cancellationToken = default);

    Task UpdateEnrolledCountAsync(long id, int count, CancellationToken cancellationToken = default);

    
    Task<int> CountActiveByProgramCodeAsync(string programCode, CancellationToken cancellationToken = default);
}