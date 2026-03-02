using EvuEase.Application.Common;
using EvuEase.Application.DTOs;
using EvuEase.Application.DTOs.Curricula;
using EvuEase.Domain.Entities;

namespace EvuEase.Application.Interfaces.Repositories;

public interface ICurriculaRepository
{
    Task<PagedResults<Curricula>> GetAllCurricula(CurriculaRequest curriculaRequest);
    Task<Curricula?> GetCurriculaByIdAsync(long id);
    Task<Curricula?> GetCurriculaByCodeAsync(string curriculumCode);
    Task<Curricula> CreateCurriculaAsync(Curricula curricula);
    Task<Curricula> UpdateCurriculaAsync(Curricula curricula);
    Task DeleteCurriculaAsync(Curricula curricula);
    Task<List<LookupItem>> GetLookupItemsAsync(long? programId = null);
    Task<List<LookupItem>> GetCurriculumVersionsByProgramCodeAsync(string programCode);
}

