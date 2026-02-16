using EvuEase.Application.Common;
using EvuEase.Application.DTOs.Program;
using EvuEase.Domain.Entities;

namespace EvuEase.Application.Interfaces.Repositories;

public interface IProgramRepository
{
    Task<PagedResults<Program>> GetAllPrograms(ProgramRequest programRequest);
    Task<Program?> GetProgramByIdAsync(long id);
    Task<Program?> GetProgramByCodeAsync(string programCode);
    Task<Program> CreateProgramAsync(Program program);
    Task<Program> UpdateProgramAsync(Program program);
    Task DeleteProgramAsync(Program program);
    Task<bool> ProgramCodeExistsAsync(string programCode, long? excludeProgramId = null);
}


