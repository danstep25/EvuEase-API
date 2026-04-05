using EvuEase.Application.Common;
using EvuEase.Application.DTOs.Downpayment;

namespace EvuEase.Application.Interfaces.Services;

public interface IDownpaymentService
{
    Task<PagedResults<DownpaymentResponse>> GetAllAsync(DownpaymentRequest request);
    Task<DownpaymentResponse?> GetByIdAsync(long id);
    Task<DownpaymentResponse> CreateAsync(CreateDownpaymentRequest request);
    Task<DownpaymentResponse> UpdateAsync(UpdateDownpaymentRequest request);
    Task DeleteAsync(long id);
    Task<IReadOnlyList<DownpaymentResponse>> GetHistoryByProgramCodeAsync(string programCode);
}
