using EvuEase.Application.Common;
using EvuEase.Application.DTOs;
using EvuEase.Application.DTOs.SyTerm;
using EvuEase.Application.Interfaces.Repositories;
using EvuEase.Domain.Entities;
using EvuEase.Infrastructure.Common;
using EvuEase.Infrastructure.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace EvuEase.Infrastructure.Repositories;

public class SyTermRepository : BaseRepository<SyTerm>, ISyTermRepository
{
    public SyTermRepository(AppDbContext dbContext, IHttpContextAccessor httpContextAccessor)
        : base(dbContext, httpContextAccessor) { }

    public async Task<PagedResults<SyTerm>> GetAllSyTerms(SyTermRequest syTermRequest)
    {
        var query = GetAll();

        if (!string.IsNullOrWhiteSpace(syTermRequest.SearchTerm))
        {
            var searchTerm = syTermRequest.SearchTerm.ToLower();
            query = query.Where(s =>
                s.sy_code.ToLower().Contains(searchTerm) ||
                s.sy_year.ToLower().Contains(searchTerm) ||
                s.sy_semester.ToLower().Contains(searchTerm)
            );
        }

        if (!string.IsNullOrWhiteSpace(syTermRequest.SyCode))
        {
            query = query.Where(s => s.sy_code.ToLower().Contains(syTermRequest.SyCode.ToLower()));
        }

        if (!string.IsNullOrWhiteSpace(syTermRequest.SyYear))
        {
            query = query.Where(s => s.sy_year.ToLower().Contains(syTermRequest.SyYear.ToLower()));
        }

        if (!string.IsNullOrWhiteSpace(syTermRequest.Semester))
        {
            query = query.Where(s => s.sy_semester.ToLower().Contains(syTermRequest.Semester.ToLower()));
        }

        if (!string.IsNullOrWhiteSpace(syTermRequest.Status))
        {
            query = query.Where(s => s.sy_status.ToLower() == syTermRequest.Status.ToLower());
        }

        return await query.PaginateAsync(
            syTermRequest.PageIndex,
            syTermRequest.PageSize,
            syTermRequest.SortKey,
            syTermRequest.SortDirection
        );
    }

    public async Task<SyTerm?> GetSyTermByIdAsync(long id)
    {
        return await GetAll().FirstOrDefaultAsync(s => s.sy_id == id);
    }

    public async Task<SyTerm?> GetCurrentSyTermAsync(CancellationToken cancellationToken = default)
    {
        return await GetAll()
            .Where(s => s.sy_status.ToLower() == "active")
            .OrderByDescending(s => s.sy_year)
            .ThenByDescending(s => s.sy_semester)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<SyTerm> SetCurrentSyTermAsync(long syTermId, CancellationToken cancellationToken = default)
    {
        var target = await GetSyTermByIdAsync(syTermId);
        if (target == null)
        {
            throw new Exception("School Year Term not found");
        }

        var activeOthers = await GetAll()
            .Where(s => s.sy_id != syTermId && s.sy_status.ToLower() == "active")
            .ToListAsync(cancellationToken);

        foreach (var row in activeOthers)
        {
            row.Update(
                row.sy_code,
                row.sy_year,
                row.sy_semester,
                row.sy_startdate,
                row.sy_enddate,
                row.sy_enrollmentstart,
                row.sy_enrollmentend,
                "Inactive");
            await UpdateAsync(row, cancellationToken);
        }

        if (!string.Equals(target.sy_status, "Active", StringComparison.OrdinalIgnoreCase))
        {
            target.Update(
                target.sy_code,
                target.sy_year,
                target.sy_semester,
                target.sy_startdate,
                target.sy_enddate,
                target.sy_enrollmentstart,
                target.sy_enrollmentend,
                "Active");
            await UpdateAsync(target, cancellationToken);
        }

        await SaveChangesAsync(cancellationToken);
        return target;
    }

    public async Task<SyTerm> CreateSyTermAsync(SyTerm syTerm)
    {
        await AddAsync(syTerm);
        await SaveChangesAsync();
        return syTerm;
    }

    public async Task<SyTerm> UpdateSyTermAsync(SyTerm syTerm)
    {
        await UpdateAsync(syTerm);
        await SaveChangesAsync();
        return syTerm;
    }

    public async Task DeleteSyTermAsync(SyTerm syTerm)
    {
        await SoftDeleteAsync(syTerm);
        await SaveChangesAsync();
    }

    public async Task<List<LookupItem>> GetLookupItemsAsync()
    {
        return await GetAll()
            .OrderByDescending(s => s.sy_year)
            .ThenByDescending(s => s.sy_semester)
            .Select(s => new LookupItem
            {
                Id = s.sy_id,
                Value = s.sy_code,
                DisplayText = $"{s.sy_year} - {s.sy_semester}"
            })
            .ToListAsync();
    }
}

