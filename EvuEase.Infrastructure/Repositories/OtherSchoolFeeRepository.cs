using EvuEase.Application.Common;
using EvuEase.Application.DTOs.OtherSchoolFee;
using EvuEase.Application.Interfaces.Repositories;
using EvuEase.Domain.Entities;
using EvuEase.Infrastructure.Common;
using EvuEase.Infrastructure.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace EvuEase.Infrastructure.Repositories;

public class OtherSchoolFeeRepository : BaseRepository<OtherSchoolFee>, IOtherSchoolFeeRepository
{
    public OtherSchoolFeeRepository(AppDbContext dbContext, IHttpContextAccessor httpContextAccessor)
        : base(dbContext, httpContextAccessor) { }

    public async Task<PagedResults<OtherSchoolFee>> GetAllOtherSchoolFees(OtherSchoolFeeRequest request)
    {
        var query = GetAll();

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var searchTerm = request.SearchTerm.ToLower();
            query = query.Where(f =>
                (f.school_fee != null && f.school_fee.ToLower().Contains(searchTerm)) ||
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

        if (!string.IsNullOrWhiteSpace(request.SchoolFee))
        {
            query = query.Where(f => f.school_fee != null && f.school_fee.ToLower().Contains(request.SchoolFee.ToLower()));
        }

        return await query.PaginateAsync(
            request.PageIndex,
            request.PageSize,
            request.SortKey,
            request.SortDirection
        );
    }

    public async Task<OtherSchoolFee?> GetOtherSchoolFeeByIdAsync(long id)
    {
        return await GetAll().FirstOrDefaultAsync(f => f.id == id);
    }

    public async Task<OtherSchoolFee> CreateOtherSchoolFeeAsync(OtherSchoolFee fee)
    {
        await AddAsync(fee);
        await SaveChangesAsync();
        return fee;
    }

    public async Task<OtherSchoolFee> UpdateOtherSchoolFeeAsync(OtherSchoolFee fee)
    {
        await UpdateAsync(fee);
        await SaveChangesAsync();
        return fee;
    }

    public async Task DeleteOtherSchoolFeeAsync(OtherSchoolFee fee)
    {
        await SoftDeleteAsync(fee);
        await SaveChangesAsync();
    }
}



