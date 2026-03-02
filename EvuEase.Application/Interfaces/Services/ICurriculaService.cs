using EvuEase.Application.Common;
using EvuEase.Application.DTOs.Curricula;

namespace EvuEase.Application.Interfaces.Services;

public interface ICurriculaService
{
    Task<PagedResults<CurriculaResponse>> GetAllCurricula(CurriculaRequest curriculaRequest);
    Task<CurriculaResponse?> GetCurriculaByIdAsync(long id);
    Task<CurriculaResponse> CreateCurriculaAsync(CreateCurriculaRequest curriculaRequest);
    Task<CurriculaResponse> UpdateCurriculaAsync(UpdateCurriculaRequest curriculaRequest);
    Task DeleteCurriculaAsync(long id);
}



