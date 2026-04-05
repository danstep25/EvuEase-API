using EvuEase.Application.Interfaces.Repositories;
using EvuEase.Domain.Entities;
using EvuEase.Infrastructure.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace EvuEase.Infrastructure.Repositories;

public class GradingSchemeBasisRepository : BaseRepository<GradingSchemeBasis>, IGradingSchemeBasisRepository
{
    public GradingSchemeBasisRepository(AppDbContext dbContext, IHttpContextAccessor httpContextAccessor)
        : base(dbContext, httpContextAccessor) { }

    public async Task<GradingSchemeBasis?> GetByAcademicTermKeyAsync(
        string academicTermKey,
        CancellationToken cancellationToken = default)
    {
        return await GetAll()
            .FirstOrDefaultAsync(
                e => e.academic_term_key == academicTermKey,
                cancellationToken);
    }

    public async Task<GradingSchemeBasis> CreateGradingSchemeBasisAsync(GradingSchemeBasis entity, CancellationToken cancellationToken = default)
    {
        await AddAsync(entity, cancellationToken);
        await SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task<GradingSchemeBasis> UpdateGradingSchemeBasisAsync(GradingSchemeBasis entity, CancellationToken cancellationToken = default)
    {
        await UpdateAsync(entity, cancellationToken);
        await SaveChangesAsync(cancellationToken);
        return entity;
    }
}
