using EvuEase.Application.Interfaces.Repositories;
using EvuEase.Domain.Entities;
using EvuEase.Infrastructure.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace EvuEase.Infrastructure.Repositories;

public class StudentCurriculumHistoryRepository : BaseRepository<StudentCurriculumHistory>, IStudentCurriculumHistoryRepository
{
    public StudentCurriculumHistoryRepository(AppDbContext dbContext, IHttpContextAccessor httpContextAccessor)
        : base(dbContext, httpContextAccessor)
    {
    }

    public async Task<StudentCurriculumHistory> CreateAsync(
        StudentCurriculumHistory history,
        CancellationToken cancellationToken = default)
    {
        await AddAsync(history, cancellationToken);
        await SaveChangesAsync(cancellationToken);
        return history;
    }

    public async Task<IReadOnlyList<StudentCurriculumHistory>> GetByStudentIdAsync(
        long studentId,
        CancellationToken cancellationToken = default)
    {
        return await dbContext.StudentCurriculumHistories
            .AsNoTracking()
            .Where(h => h.student_id == studentId)
            .OrderByDescending(h => h.created_at)
            .ToListAsync(cancellationToken);
    }
}
