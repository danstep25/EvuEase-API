using System.Security.Claims;
using EvuEase.Application.Common;
using EvuEase.Application.DTOs.Downpayment;
using EvuEase.Application.Interfaces.Repositories;
using EvuEase.Domain.Entities;
using EvuEase.Infrastructure.Common;
using EvuEase.Infrastructure.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace EvuEase.Infrastructure.Repositories;

public class DownpaymentRepository : BaseRepository<DpPercentage>, IDownpaymentRepository
{
    public DownpaymentRepository(AppDbContext dbContext, IHttpContextAccessor httpContextAccessor)
        : base(dbContext, httpContextAccessor)
    {
    }

    public async Task<PagedResults<DpPercentage>> GetAllAsync(DownpaymentRequest request)
    {
        var query = GetAll();

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var s = request.SearchTerm.Trim().ToLower();
            query = query.Where(d =>
                (d.program_code != null && d.program_code.ToLower().Contains(s)) ||
                (d.program_title != null && d.program_title.ToLower().Contains(s)) ||
                (d.batch != null && d.batch.ToLower().Contains(s)) ||
                (d.effective_school_year != null && d.effective_school_year.ToLower().Contains(s)));
        }

        return await query.PaginateAsync(
            request.PageIndex,
            request.PageSize,
            request.SortKey,
            request.SortDirection);
    }

    public async Task<DpPercentage?> GetByIdAsync(long id)
    {
        return await GetAll().FirstOrDefaultAsync(d => d.id == id);
    }

    public async Task<DpPercentage> CreateAsync(DpPercentage entity)
    {
        entity.SetCreatedBy(GetUserDisplayName());
        await AddAsync(entity);
        await SaveChangesAsync();
        return entity;
    }

    public async Task<DpPercentage> SupersedeWithNewRowAsync(DpPercentage oldRow, DpPercentage newEntity)
    {
        await using var tx = await dbContext.Database.BeginTransactionAsync();
        try
        {
            await SoftDeleteAsync(oldRow);
            newEntity.SetCreatedBy(GetUserDisplayName());
            await AddAsync(newEntity);
            await SaveChangesAsync();
            await tx.CommitAsync();
            return newEntity;
        }
        catch
        {
            await tx.RollbackAsync();
            throw;
        }
    }

    public async Task<bool> ExistsOtherActiveWithProgramCodeAsync(string programCode, long? excludeId)
    {
        var pc = programCode.Trim().ToLowerInvariant();
        var q = GetAll().Where(d =>
            d.program_code != null &&
            d.program_code.Trim().ToLower() == pc);

        if (excludeId.HasValue)
        {
            q = q.Where(d => d.id != excludeId.Value);
        }

        return await q.AnyAsync();
    }

    public async Task<IReadOnlyList<DpPercentage>> GetHistoryByProgramCodeAsync(string programCode)
    {
        if (string.IsNullOrWhiteSpace(programCode))
        {
            return Array.Empty<DpPercentage>();
        }

        var code = programCode.Trim();
        var lower = code.ToLowerInvariant();
        var query = GetAll(includeDeleted: true).Where(d =>
            d.program_code != null && d.program_code.Trim().ToLower() == lower);

        var list = await query.ToListAsync();
        return list
            .OrderByDescending(d => d.updated_at ?? d.created_at ?? DateTime.MinValue)
            .ToList();
    }

    private string? GetUserDisplayName()
    {
        var user = httpContextAccessor?.HttpContext?.User;
        if (user?.Identity is not { IsAuthenticated: true })
        {
            return null;
        }

        return user.FindFirst("UserName")?.Value
            ?? user.FindFirst(ClaimTypes.Email)?.Value
            ?? user.FindFirst("Email")?.Value
            ?? user.FindFirst("UserId")?.Value
            ?? user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    }

    public async Task DeleteAsync(DpPercentage entity)
    {
        await SoftDeleteAsync(entity);
        await SaveChangesAsync();
    }
}
