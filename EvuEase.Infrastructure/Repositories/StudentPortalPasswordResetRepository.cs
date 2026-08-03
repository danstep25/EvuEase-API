using EvuEase.Application.Interfaces.Repositories;
using EvuEase.Domain.Entities;
using EvuEase.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EvuEase.Infrastructure.Repositories;

public class StudentPortalPasswordResetRepository : IStudentPortalPasswordResetRepository
{
    private readonly AppDbContext _dbContext;

    public StudentPortalPasswordResetRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<StudentPortalPasswordResetRequest> CreateAsync(
        StudentPortalPasswordResetRequest request,
        CancellationToken cancellationToken = default)
    {
        await _dbContext.StudentPortalPasswordResetRequests.AddAsync(request, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return request;
    }

    public async Task<StudentPortalPasswordResetRequest?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.StudentPortalPasswordResetRequests
            .FirstOrDefaultAsync(r => r.id == id, cancellationToken);
    }

    public async Task<bool> HasPendingForStudentAsync(long studentId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.StudentPortalPasswordResetRequests.AnyAsync(
            r => r.student_id == studentId
                && (r.status == StudentPortalPasswordResetRequest.StatusPending
                    || r.status == StudentPortalPasswordResetRequest.StatusTempIssued),
            cancellationToken);
    }

    public async Task<StudentPortalPasswordResetRequest?> GetActiveTempIssuedForStudentAsync(
        long studentId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.StudentPortalPasswordResetRequests
            .Where(r => r.student_id == studentId
                && r.status == StudentPortalPasswordResetRequest.StatusTempIssued)
            .OrderByDescending(r => r.requested_at)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<StudentPortalPasswordResetRequest>> GetAllAsync(
        string? status,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.StudentPortalPasswordResetRequests.AsQueryable();
        if (!string.IsNullOrWhiteSpace(status))
        {
            query = query.Where(r => r.status == status.Trim());
        }

        return await query
            .OrderByDescending(r => r.requested_at)
            .ToListAsync(cancellationToken);
    }

    public async Task UpdateAsync(StudentPortalPasswordResetRequest request, CancellationToken cancellationToken = default)
    {
        _dbContext.StudentPortalPasswordResetRequests.Update(request);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
