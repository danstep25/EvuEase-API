using EvuEase.Application.Common;
using EvuEase.Application.DTOs.PaymentScheme;
using EvuEase.Application.Interfaces.Repositories;
using EvuEase.Domain.Entities;
using EvuEase.Infrastructure.Common;
using EvuEase.Infrastructure.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace EvuEase.Infrastructure.Repositories;

public class PaymentSchemeRepository : BaseRepository<PaymentScheme>, IPaymentSchemeRepository
{
    public PaymentSchemeRepository(AppDbContext dbContext, IHttpContextAccessor httpContextAccessor)
        : base(dbContext, httpContextAccessor)
    {
    }

    public async Task<PagedResults<PaymentScheme>> GetAllAsync(PaymentSchemeRequest request)
    {
        var query = GetAll();

        if (!string.IsNullOrWhiteSpace(request.SchoolYear))
        {
            var sy = request.SchoolYear.Trim();
            query = query.Where(s => s.school_year != null && s.school_year.Trim() == sy);
        }

        if (!string.IsNullOrWhiteSpace(request.Semester))
        {
            var sem = request.Semester.Trim();
            query = query.Where(s => s.semester != null && s.semester.Trim() == sem);
        }

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var s = request.SearchTerm.Trim().ToLower();
            query = query.Where(scheme =>
                (scheme.school_year != null && scheme.school_year.ToLower().Contains(s)) ||
                (scheme.semester != null && scheme.semester.ToLower().Contains(s)) ||
                (scheme.description != null && scheme.description.ToLower().Contains(s)));
        }

        return await query.PaginateAsync(
            request.PageIndex,
            request.PageSize,
            request.SortKey,
            request.SortDirection);
    }

    public async Task<PaymentScheme?> GetByIdAsync(long id)
    {
        return await GetAll().FirstOrDefaultAsync(s => s.id == id);
    }

    public async Task<List<PaymentSchemeInstallment>> GetInstallmentsBySchemeIdsAsync(IEnumerable<long> schemeIds)
    {
        var ids = schemeIds.Distinct().ToList();
        if (ids.Count == 0)
        {
            return new List<PaymentSchemeInstallment>();
        }

        return await dbContext.Set<PaymentSchemeInstallment>()
            .Where(i =>
                ids.Contains(i.payment_scheme_id) &&
                i.deleted_at == null &&
                i.status)
            .OrderBy(i => i.payment_scheme_id)
            .ThenBy(i => i.installment_order)
            .ToListAsync();
    }

    public async Task<PaymentScheme> CreateAsync(PaymentScheme scheme, IEnumerable<PaymentSchemeInstallment> installments)
    {
        await using var tx = await dbContext.Database.BeginTransactionAsync();
        try
        {
            await AddAsync(scheme);
            await SaveChangesAsync();

            foreach (var installment in installments)
            {
                var created = PaymentSchemeInstallment.Create(
                    scheme.id,
                    installment.installment_order,
                    installment.payment_name,
                    installment.due_date);
                await dbContext.Set<PaymentSchemeInstallment>().AddAsync(created);
            }

            await SaveChangesAsync();
            await tx.CommitAsync();
            return scheme;
        }
        catch
        {
            await tx.RollbackAsync();
            throw;
        }
    }

    public async Task<PaymentScheme> UpdateAsync(PaymentScheme scheme, IEnumerable<PaymentSchemeInstallment> installments)
    {
        await using var tx = await dbContext.Database.BeginTransactionAsync();
        try
        {
            await base.UpdateAsync(scheme);
            await SaveChangesAsync();

            var existing = await dbContext.Set<PaymentSchemeInstallment>()
                .Where(i => i.payment_scheme_id == scheme.id && i.deleted_at == null && i.status)
                .ToListAsync();

            foreach (var row in existing)
            {
                await SoftDeleteInstallmentAsync(row);
            }

            foreach (var installment in installments)
            {
                var created = PaymentSchemeInstallment.Create(
                    scheme.id,
                    installment.installment_order,
                    installment.payment_name,
                    installment.due_date);
                await dbContext.Set<PaymentSchemeInstallment>().AddAsync(created);
            }

            await SaveChangesAsync();
            await tx.CommitAsync();
            return scheme;
        }
        catch
        {
            await tx.RollbackAsync();
            throw;
        }
    }

    public async Task DeleteAsync(PaymentScheme scheme)
    {
        await using var tx = await dbContext.Database.BeginTransactionAsync();
        try
        {
            var installments = await dbContext.Set<PaymentSchemeInstallment>()
                .Where(i => i.payment_scheme_id == scheme.id && i.deleted_at == null && i.status)
                .ToListAsync();

            foreach (var row in installments)
            {
                await SoftDeleteInstallmentAsync(row);
            }

            await SoftDeleteAsync(scheme);
            await SaveChangesAsync();
            await tx.CommitAsync();
        }
        catch
        {
            await tx.RollbackAsync();
            throw;
        }
    }

    public async Task<bool> ExistsOtherActiveAsync(string schoolYear, string semester, long? excludeId)
    {
        var sy = schoolYear.Trim().ToLowerInvariant();
        var sem = semester.Trim().ToLowerInvariant();
        var q = GetAll().Where(s =>
            s.school_year != null &&
            s.semester != null &&
            s.school_year.Trim().ToLower() == sy &&
            s.semester.Trim().ToLower() == sem);

        if (excludeId.HasValue)
        {
            q = q.Where(s => s.id != excludeId.Value);
        }

        return await q.AnyAsync();
    }

    private async Task SoftDeleteInstallmentAsync(PaymentSchemeInstallment installment)
    {
        var type = typeof(BaseEntity);
        type.GetProperty(nameof(BaseEntity.status))?.SetValue(installment, false);
        type.GetProperty(nameof(BaseEntity.updated_at))?.SetValue(installment, DateTime.UtcNow);
        type.GetProperty(nameof(BaseEntity.deleted_at))?.SetValue(installment, DateTime.UtcNow);
        dbContext.Set<PaymentSchemeInstallment>().Update(installment);
        await Task.CompletedTask;
    }
}
