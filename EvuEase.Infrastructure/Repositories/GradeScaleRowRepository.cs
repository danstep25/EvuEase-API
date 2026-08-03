using EvuEase.Application.DTOs.FacultyCenter;
using EvuEase.Application.Interfaces.Repositories;
using EvuEase.Domain.Entities;
using EvuEase.Infrastructure.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace EvuEase.Infrastructure.Repositories;

public class GradeScaleRowRepository : BaseRepository<GradeScaleRow>, IGradeScaleRowRepository
{
    public GradeScaleRowRepository(AppDbContext dbContext, IHttpContextAccessor httpContextAccessor)
        : base(dbContext, httpContextAccessor) { }

    public async Task<IReadOnlyList<GradeScaleRow>> GetByAcademicTermKeyAsync(
        string academicTermKey,
        CancellationToken cancellationToken = default)
    {
        return await GetAll()
            .Where(e => e.academic_term_key == academicTermKey)
            .OrderBy(e => e.sort_order)
            .ToListAsync(cancellationToken);
    }

    public async Task ReplaceForAcademicTermAsync(
        string academicTermKey,
        IReadOnlyList<GradeScaleRowRequest> rows,
        CancellationToken cancellationToken = default)
    {
        var existing = await dbContext.Set<GradeScaleRow>()
            .Where(e => e.academic_term_key == academicTermKey)
            .ToListAsync(cancellationToken);

        if (existing.Count > 0)
        {
            dbContext.Set<GradeScaleRow>().RemoveRange(existing);
        }

        for (var i = 0; i < rows.Count; i++)
        {
            var r = rows[i];
            var entity = GradeScaleRow.Create(academicTermKey, r.Mark, r.Grade, i);
            await AddAsync(entity, cancellationToken);
        }

        await SaveChangesAsync(cancellationToken);
    }
}
