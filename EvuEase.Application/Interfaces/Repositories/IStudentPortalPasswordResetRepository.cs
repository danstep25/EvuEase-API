using EvuEase.Domain.Entities;

namespace EvuEase.Application.Interfaces.Repositories;

public interface IStudentPortalPasswordResetRepository
{
    Task<StudentPortalPasswordResetRequest> CreateAsync(StudentPortalPasswordResetRequest request, CancellationToken cancellationToken = default);
    Task<StudentPortalPasswordResetRequest?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<bool> HasPendingForStudentAsync(long studentId, CancellationToken cancellationToken = default);
    Task<StudentPortalPasswordResetRequest?> GetActiveTempIssuedForStudentAsync(long studentId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<StudentPortalPasswordResetRequest>> GetAllAsync(string? status, CancellationToken cancellationToken = default);
    Task UpdateAsync(StudentPortalPasswordResetRequest request, CancellationToken cancellationToken = default);
}
