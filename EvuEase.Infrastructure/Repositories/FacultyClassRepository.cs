using EvuEase.Application.Interfaces.Repositories;
using EvuEase.Domain.Entities;
using EvuEase.Infrastructure.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace EvuEase.Infrastructure.Repositories;

public class FacultyClassRepository : BaseRepository<FacultyClass>, IFacultyClassRepository
{
    public FacultyClassRepository(AppDbContext dbContext, IHttpContextAccessor httpContextAccessor)
        : base(dbContext, httpContextAccessor) { }

    public async Task<IReadOnlyList<FacultyClass>> GetAllAsync(string? search, CancellationToken cancellationToken = default)
    {
        var query = GetAll().AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim().ToLower();
            query = query.Where(e =>
                e.course_code.ToLower().Contains(s) ||
                e.class_number.ToLower().Contains(s) ||
                e.section.ToLower().Contains(s) ||
                e.course_title.ToLower().Contains(s) ||
                e.component.ToLower().Contains(s) ||
                e.academic_term.ToLower().Contains(s) ||
                e.program_code.ToLower().Contains(s) ||
                e.year_level.ToLower().Contains(s));
        }

        return await query
            .OrderBy(e => e.course_code)
            .ThenBy(e => e.class_number)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<FacultyClass>> GetForGradeRosterLookupAsync(
        IReadOnlyList<string> academicTermMatchKeys,
        string? search,
        CancellationToken cancellationToken = default)
    {
        if (academicTermMatchKeys == null || academicTermMatchKeys.Count == 0)
        {
            return Array.Empty<FacultyClass>();
        }

        var query = GetAll().AsQueryable().Where(e => academicTermMatchKeys.Contains(e.academic_term));

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim().ToLower();
            query = query.Where(e =>
                e.course_code.ToLower().Contains(s) ||
                e.course_title.ToLower().Contains(s));
        }

        return await query
            .OrderBy(e => e.course_code)
            .ThenBy(e => e.class_number)
            .ThenBy(e => e.section)
            .ToListAsync(cancellationToken);
    }

    public async Task<FacultyClass?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        return await GetAll().FirstOrDefaultAsync(e => e.id == id, cancellationToken);
    }

    public async Task<FacultyClass> CreateAsync(FacultyClass entity, CancellationToken cancellationToken = default)
    {
        await AddAsync(entity, cancellationToken);
        await SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task SoftDeleteFacultyClassAsync(FacultyClass entity, CancellationToken cancellationToken = default)
    {
        await SoftDeleteAsync(entity, cancellationToken);
        await SaveChangesAsync(cancellationToken);
    }

    public async Task<int> CountActiveByProgramCodeAsync(string programCode, CancellationToken cancellationToken = default)
    {
        var code = programCode.Trim();
        return await GetAll()
            .Where(f => f.program_code.ToLower() == code.ToLower())
            .CountAsync(cancellationToken);
    }

    public async Task UpdateEnrolledCountAsync(long id, int count, CancellationToken cancellationToken = default)
    {
        var entity = await GetByIdAsync(id, cancellationToken);
        if (entity == null)
        {
            throw new KeyNotFoundException($"Class with ID {id} was not found.");
        }

        entity.SetEnrolledCount(count);
        dbContext.Set<FacultyClass>().Update(entity);
        await SaveChangesAsync(cancellationToken);
    }
}
