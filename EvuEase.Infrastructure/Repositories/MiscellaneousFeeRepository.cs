using EvuEase.Application.Common;
using EvuEase.Application.DTOs.MiscellaneousFee;
using EvuEase.Application.Interfaces.Repositories;
using EvuEase.Domain.Entities;
using EvuEase.Infrastructure.Common;
using EvuEase.Infrastructure.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace EvuEase.Infrastructure.Repositories;

public class MiscellaneousFeeRepository : BaseRepository<MiscellaneousFee>, IMiscellaneousFeeRepository
{
    public MiscellaneousFeeRepository(AppDbContext dbContext, IHttpContextAccessor httpContextAccessor)
        : base(dbContext, httpContextAccessor) { }

    public async Task<PagedResults<MiscellaneousFee>> GetAllMiscellaneousFees(MiscellaneousFeeRequest request)
    {
        var query = GetAll();

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var searchTerm = request.SearchTerm.ToLower();
            query = query.Where(f =>
                (f.miscellaneous_fee != null && f.miscellaneous_fee.ToLower().Contains(searchTerm)) ||
                (f.batch != null && f.batch.ToLower().Contains(searchTerm)) ||
                (f.semester != null && f.semester.ToLower().Contains(searchTerm))
            );
        }

        if (!string.IsNullOrWhiteSpace(request.SyId))
        {
            query = query.Where(f => f.sy_id != null && f.sy_id.ToLower().Contains(request.SyId.ToLower()));
        }

        if (!string.IsNullOrWhiteSpace(request.Batch))
        {
            query = query.Where(f => f.batch != null && f.batch.ToLower().Contains(request.Batch.ToLower()));
        }

        if (!string.IsNullOrWhiteSpace(request.Semester))
        {
            query = query.Where(f => f.semester != null && f.semester.ToLower() == request.Semester.ToLower());
        }

        if (!string.IsNullOrWhiteSpace(request.MiscellaneousFee))
        {
            query = query.Where(f => f.miscellaneous_fee != null && f.miscellaneous_fee.ToLower().Contains(request.MiscellaneousFee.ToLower()));
        }

        return await query.PaginateAsync(
            request.PageIndex,
            request.PageSize,
            request.SortKey,
            request.SortDirection
        );
    }

    public async Task<MiscellaneousFee?> GetMiscellaneousFeeByIdAsync(long id)
    {
        return await GetAll().FirstOrDefaultAsync(f => f.id == id);
    }

    public async Task<MiscellaneousFee> CreateMiscellaneousFeeAsync(MiscellaneousFee fee)
    {
        await AddAsync(fee);
        await SaveChangesAsync();
        return fee;
    }

    public async Task<MiscellaneousFee> UpdateMiscellaneousFeeAsync(MiscellaneousFee fee)
    {
        await UpdateAsync(fee);
        await SaveChangesAsync();
        return fee;
    }

    public async Task DeleteMiscellaneousFeeAsync(MiscellaneousFee fee)
    {
        await SoftDeleteAsync(fee);
        await SaveChangesAsync();
    }
}



