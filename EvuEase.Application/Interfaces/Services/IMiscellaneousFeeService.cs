using EvuEase.Application.Common;
using EvuEase.Application.DTOs.MiscellaneousFee;

namespace EvuEase.Application.Interfaces.Services;

public interface IMiscellaneousFeeService
{
    Task<PagedResults<MiscellaneousFeeResponse>> GetAllMiscellaneousFees(MiscellaneousFeeRequest request);
    Task<MiscellaneousFeeResponse?> GetMiscellaneousFeeByIdAsync(long id);
    Task<MiscellaneousFeeResponse> CreateMiscellaneousFeeAsync(CreateMiscellaneousFeeRequest request);
    Task<MiscellaneousFeeResponse> UpdateMiscellaneousFeeAsync(UpdateMiscellaneousFeeRequest request);
    Task DeleteMiscellaneousFeeAsync(long id);
}


