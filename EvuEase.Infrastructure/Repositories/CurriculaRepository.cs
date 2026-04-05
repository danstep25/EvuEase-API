using EvuEase.Application.Common;
using EvuEase.Application.DTOs;
using EvuEase.Application.DTOs.Curricula;
using EvuEase.Application.Interfaces.Repositories;
using EvuEase.Domain.Entities;
using EvuEase.Infrastructure.Common;
using EvuEase.Infrastructure.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace EvuEase.Infrastructure.Repositories;

public class CurriculaRepository : BaseRepository<Curricula>, ICurriculaRepository
{
    public CurriculaRepository(AppDbContext dbContext, IHttpContextAccessor httpContextAccessor)
        : base(dbContext, httpContextAccessor) { }

    public async Task<PagedResults<Curricula>> GetAllCurricula(CurriculaRequest curriculaRequest)
    {
        var query = GetAll();

        if (!string.IsNullOrWhiteSpace(curriculaRequest.SearchTerm))
        {
            var searchTerm = curriculaRequest.SearchTerm.ToLower();
            query = query.Where(c =>
                c.curriculum_code.ToLower().Contains(searchTerm) ||
                c.version.ToLower().Contains(searchTerm)
            );
        }

        if (!string.IsNullOrWhiteSpace(curriculaRequest.CurriculumCode))
        {
            query = query.Where(c => c.curriculum_code.ToLower().Contains(curriculaRequest.CurriculumCode.ToLower()));
        }

        if (!string.IsNullOrWhiteSpace(curriculaRequest.Version))
        {
            query = query.Where(c => c.version.ToLower().Contains(curriculaRequest.Version.ToLower()));
        }

        if (curriculaRequest.ProgramId.HasValue)
        {
            query = query.Where(c => c.program_id == curriculaRequest.ProgramId.Value);
        }

        if (curriculaRequest.SyId.HasValue)
        {
            query = query.Where(c => c.sy_id == curriculaRequest.SyId.Value);
        }

        if (!string.IsNullOrWhiteSpace(curriculaRequest.Status))
        {
            query = query.Where(c => c.curriculum_status.ToLower() == curriculaRequest.Status.ToLower());
        }

        return await query.PaginateAsync(
            curriculaRequest.PageIndex,
            curriculaRequest.PageSize,
            curriculaRequest.SortKey,
            curriculaRequest.SortDirection
        );
    }

    public async Task<Curricula?> GetCurriculaByIdAsync(long id)
    {
        return await GetAll().FirstOrDefaultAsync(c => c.id == id);
    }

    public async Task<Curricula?> GetCurriculaByCodeAsync(string curriculumCode)
    {
        return await GetAll().FirstOrDefaultAsync(c => c.curriculum_code == curriculumCode);
    }

    public async Task<Curricula> CreateCurriculaAsync(Curricula curricula)
    {
        await AddAsync(curricula);
        await SaveChangesAsync();
        return curricula;
    }

    public async Task<Curricula> UpdateCurriculaAsync(Curricula curricula)
    {
        await UpdateAsync(curricula);
        await SaveChangesAsync();
        return curricula;
    }

    public async Task DeleteCurriculaAsync(Curricula curricula)
    {
        await SoftDeleteAsync(curricula);
        await SaveChangesAsync();
    }

    public async Task<List<LookupItem>> GetLookupItemsAsync(long? programId = null)
    {
        var query = GetAll();

        if (programId.HasValue)
        {
            query = query.Where(c => c.program_id == programId.Value);
        }

        return await query
            .Select(c => new LookupItem
            {
                Id = c.id,
                Value = c.curriculum_code,
                DisplayText = $"{c.curriculum_code} - {c.version}"
            })
            .OrderBy(c => c.Value)
            .ToListAsync();
    }

    public async Task<List<LookupItem>> GetCurriculumVersionsByProgramCodeAsync(string programCode)
    {
        if (string.IsNullOrWhiteSpace(programCode))
        {
            return new List<LookupItem>();
        }

        return await GetAll()
            .Where(c => c.curriculum_code.StartsWith(programCode + "-"))
            .Select(c => new LookupItem
            {
                Id = c.id,
                Value = c.version,
                DisplayText = $"{c.curriculum_code} - {c.version}"
            })
            .OrderByDescending(c => c.Value)
            .ToListAsync();
    }

    public async Task SoftDeleteAllForProgramAsync(long programId, CancellationToken cancellationToken = default)
    {
        var list = await GetAll()
            .Where(c => c.program_id == programId)
            .ToListAsync(cancellationToken);
        foreach (var row in list)
        {
            await SoftDeleteAsync(row, cancellationToken);
        }

        await SaveChangesAsync(cancellationToken);
    }
}

