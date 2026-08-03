using EvuEase.Application.DTOs.CreditRequest;
using EvuEase.Application.Interfaces.Repositories;
using EvuEase.Domain.Entities;
using EvuEase.Infrastructure.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace EvuEase.Infrastructure.Repositories;

public class CreditRequestRepository : BaseRepository<CreditRequest>, ICreditRequestRepository
{
    public CreditRequestRepository(AppDbContext dbContext, IHttpContextAccessor httpContextAccessor)
        : base(dbContext, httpContextAccessor) { }

    public async Task<IReadOnlyList<CreditRequest>> GetAllAsync(
        CreditRequestRequest request,
        CancellationToken cancellationToken = default)
    {
        var query = ApplyFilters(GetAll().AsQueryable(), request);

        query = string.IsNullOrWhiteSpace(request.SortKey)
            ? query.OrderByDescending(e => e.created_at).ThenByDescending(e => e.id)
            : query;

        return await query
            .Skip((request.PageIndex - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> CountAllAsync(CreditRequestRequest request, CancellationToken cancellationToken = default)
    {
        return await ApplyFilters(GetAll().AsQueryable(), request).CountAsync(cancellationToken);
    }

    public async Task<CreditRequest?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        return await GetAll().FirstOrDefaultAsync(e => e.id == id, cancellationToken);
    }

    public async Task<CreditRequest> CreateAsync(CreditRequest entity, CancellationToken cancellationToken = default)
    {
        await AddAsync(entity, cancellationToken);
        await SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task<CreditRequest> UpdateCreditRequestAsync(CreditRequest entity, CancellationToken cancellationToken = default)
    {
        await base.UpdateAsync(entity, cancellationToken);
        await SaveChangesAsync(cancellationToken);
        return entity;
    }

    private static IQueryable<CreditRequest> ApplyFilters(IQueryable<CreditRequest> query, CreditRequestRequest request)
    {
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var term = request.SearchTerm.Trim().ToLower();
            query = query.Where(e =>
                e.credit_request_no.ToLower().Contains(term) ||
                e.student_number.ToLower().Contains(term) ||
                e.first_name.ToLower().Contains(term) ||
                e.last_name.ToLower().Contains(term) ||
                (e.middle_name != null && e.middle_name.ToLower().Contains(term)));
        }

        if (!string.IsNullOrWhiteSpace(request.RequestStatus))
        {
            query = query.Where(e => e.request_status == request.RequestStatus.Trim());
        }

        return query;
    }
}
