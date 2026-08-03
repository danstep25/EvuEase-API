using EvuEase.Application.Common;
using EvuEase.Application.DTOs.MiscellaneousFee;
using EvuEase.Domain.Entities;

namespace EvuEase.Application.Interfaces.Repositories;

public interface IMiscellaneousFeeRepository
{
    Task<PagedResults<MiscellaneousFee>> GetAllMiscellaneousFees(MiscellaneousFeeRequest request);
    Task<MiscellaneousFee?> GetMiscellaneousFeeByIdAsync(long id);
    Task<MiscellaneousFee> CreateMiscellaneousFeeAsync(MiscellaneousFee fee);
    Task<MiscellaneousFee> UpdateMiscellaneousFeeAsync(MiscellaneousFee fee);
    Task DeleteMiscellaneousFeeAsync(MiscellaneousFee fee);
}



