using EvuEase.Application.Common;
using EvuEase.Application.DTOs.SyTerm;
using EvuEase.Domain.Entities;

namespace EvuEase.Application.Interfaces.Repositories;

public interface ISyTermRepository
{
    Task<PagedResults<SyTerm>> GetAllSyTerms(SyTermRequest syTermRequest);
    Task<SyTerm?> GetSyTermByIdAsync(long id);
    Task<SyTerm> CreateSyTermAsync(SyTerm syTerm);
    Task<SyTerm> UpdateSyTermAsync(SyTerm syTerm);
    Task DeleteSyTermAsync(SyTerm syTerm);
}

