using EvuEase.Application.Common;
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
}

