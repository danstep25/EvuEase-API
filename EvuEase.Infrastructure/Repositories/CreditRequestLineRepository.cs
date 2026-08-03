using EvuEase.Application.DTOs.CreditRequest;
using EvuEase.Application.Interfaces.Repositories;
using EvuEase.Domain.Entities;
using EvuEase.Infrastructure.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace EvuEase.Infrastructure.Repositories;

public class CreditRequestLineRepository : BaseRepository<CreditRequestLine>, ICreditRequestLineRepository
{
    public CreditRequestLineRepository(AppDbContext dbContext, IHttpContextAccessor httpContextAccessor)
        : base(dbContext, httpContextAccessor) { }

    public async Task<IReadOnlyList<CreditRequestLine>> GetByCreditRequestIdAsync(
        long creditRequestId,
        CancellationToken cancellationToken = default)
    {
        return await GetAll()
            .Where(e => e.credit_request_id == creditRequestId)
            .OrderBy(e => e.sort_order)
            .ToListAsync(cancellationToken);
    }

    public async Task ReplaceForCreditRequestAsync(
        long creditRequestId,
        IReadOnlyList<CreateCreditRequestLineRequest> lines,
        CancellationToken cancellationToken = default)
    {
        var existing = await dbContext.Set<CreditRequestLine>()
            .Where(e => e.credit_request_id == creditRequestId)
            .ToListAsync(cancellationToken);

        if (existing.Count > 0)
        {
            dbContext.Set<CreditRequestLine>().RemoveRange(existing);
        }

        for (var i = 0; i < lines.Count; i++)
        {
            var line = lines[i];
            var entity = CreditRequestLine.Create(
                creditRequestId,
                i,
                string.IsNullOrWhiteSpace(line.AppliedCourseCode) ? null : line.AppliedCourseCode.Trim(),
                string.IsNullOrWhiteSpace(line.AppliedCourseTitle) ? null : line.AppliedCourseTitle.Trim(),
                line.AppliedLecUnits,
                line.AppliedLabUnits,
                string.IsNullOrWhiteSpace(line.Grade) ? null : line.Grade.Trim(),
                string.IsNullOrWhiteSpace(line.EquivalentCourseCode) ? null : line.EquivalentCourseCode.Trim());

            await AddAsync(entity, cancellationToken);
        }

        await SaveChangesAsync(cancellationToken);
    }
}
