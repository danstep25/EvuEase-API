using EvuEase.Application.Common;
using EvuEase.Application.DTOs;
using EvuEase.Application.DTOs.SyTerm;
using EvuEase.Domain.Entities;

namespace EvuEase.Application.Interfaces.Repositories;

public interface ISyTermRepository
{
    Task<PagedResults<SyTerm>> GetAllSyTerms(SyTermRequest syTermRequest);
    Task<SyTerm?> GetSyTermByIdAsync(long id);
    Task<SyTerm?> GetCurrentSyTermAsync(CancellationToken cancellationToken = default);
    Task<SyTerm> SetCurrentSyTermAsync(long syTermId, CancellationToken cancellationToken = default);
    Task<SyTerm> CreateSyTermAsync(SyTerm syTerm);
    Task<SyTerm> UpdateSyTermAsync(SyTerm syTerm);
    Task DeleteSyTermAsync(SyTerm syTerm);
    Task<List<LookupItem>> GetLookupItemsAsync();
}

