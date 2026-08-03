using EvuEase.Application.Common;
using EvuEase.Application.DTOs.SyTerm;

namespace EvuEase.Application.Interfaces.Services;

public interface ISyTermService
{
    Task<PagedResults<SyTermResponse>> GetAllSyTerms(SyTermRequest syTermRequest);
    Task<SyTermResponse?> GetSyTermByIdAsync(long id);
    Task<SyTermResponse?> GetCurrentSyTermAsync(CancellationToken cancellationToken = default);
    Task<SyTermResponse> SetCurrentSyTermAsync(long syTermId, CancellationToken cancellationToken = default);
    Task<SyTermResponse> CreateSyTermAsync(CreateSyTermRequest syTermRequest);
    Task<SyTermResponse> UpdateSyTermAsync(UpdateSyTermRequest syTermRequest);
    Task DeleteSyTermAsync(long id);
}

