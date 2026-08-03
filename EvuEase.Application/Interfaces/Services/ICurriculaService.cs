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
    Task<CurriculaResponse?> UploadSupportingDocumentAsync(
        string curriculumCode,
        Stream content,
        string fileName,
        CancellationToken cancellationToken = default);
    Task<(Stream Stream, string FileName, string ContentType)?> GetSupportingDocumentAsync(
        string curriculumCode,
        CancellationToken cancellationToken = default);
}




