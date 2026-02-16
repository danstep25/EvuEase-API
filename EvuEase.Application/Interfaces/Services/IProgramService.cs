using EvuEase.Application.Common;
using EvuEase.Application.DTOs.Program;

namespace EvuEase.Application.Interfaces.Services;

public interface IProgramService
{
    Task<PagedResults<ProgramResponse>> GetAllPrograms(ProgramRequest programRequest);
    Task<ProgramResponse?> GetProgramByIdAsync(long id);
    Task<string> CreateProgramAsync(CreateProgramRequest programRequest);
    Task<string> UpdateProgramAsync(UpdateProgramRequest programRequest);
    Task DeleteProgramAsync(long id);
}


