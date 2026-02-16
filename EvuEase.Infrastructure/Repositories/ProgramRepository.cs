using EvuEase.Application.Common;
using EvuEase.Application.DTOs.Program;
using EvuEase.Application.Interfaces.Repositories;
using EvuEase.Domain.Entities;
using EvuEase.Infrastructure.Common;
using EvuEase.Infrastructure.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace EvuEase.Infrastructure.Repositories;

public class ProgramRepository : BaseRepository<Program>, IProgramRepository
{
    public ProgramRepository(AppDbContext dbContext, IHttpContextAccessor httpContextAccessor) 
        : base(dbContext, httpContextAccessor) { }

    public async Task<PagedResults<Program>> GetAllPrograms(ProgramRequest programRequest)
    {
        var query = GetAll();

        // Apply search filter
        if (!string.IsNullOrWhiteSpace(programRequest.SearchTerm))
        {
            var searchTerm = programRequest.SearchTerm.ToLower();
            query = query.Where(p => 
                p.program_code.ToLower().Contains(searchTerm) ||
                p.program_title.ToLower().Contains(searchTerm)
            );
        }

        if (!string.IsNullOrWhiteSpace(programRequest.ProgramCode))
        {
            query = query.Where(p => p.program_code.ToLower().Contains(programRequest.ProgramCode.ToLower()));
        }

        if (!string.IsNullOrWhiteSpace(programRequest.ProgramTitle))
        {
            query = query.Where(p => p.program_title.ToLower().Contains(programRequest.ProgramTitle.ToLower()));
        }

        if (!string.IsNullOrWhiteSpace(programRequest.Status))
        {
            query = query.Where(p => p.program_status.ToLower() == programRequest.Status.ToLower());
        }

        return await query.PaginateAsync(
            programRequest.PageIndex, 
            programRequest.PageSize, 
            programRequest.SortKey, 
            programRequest.SortDirection
        );
    }

    public async Task<Program?> GetProgramByIdAsync(long id)
    {
        return await GetAll().FirstOrDefaultAsync(p => p.program_id == id);
    }

    public async Task<Program?> GetProgramByCodeAsync(string programCode)
    {
        return await GetAll().FirstOrDefaultAsync(p => p.program_code == programCode);
    }

    public async Task<Program> CreateProgramAsync(Program program)
    {
        await AddAsync(program);
        await SaveChangesAsync();
        return program;
    }

    public async Task<Program> UpdateProgramAsync(Program program)
    {
        await UpdateAsync(program);
        await SaveChangesAsync();
        return program;
    }

    public async Task<bool> ProgramCodeExistsAsync(string programCode, long? excludeProgramId = null)
    {
        var query = GetAll().Where(p => p.program_code == programCode);
        
        if (excludeProgramId.HasValue)
        {
            query = query.Where(p => p.program_id != excludeProgramId.Value);
        }

        return await query.AnyAsync();
    }
}


