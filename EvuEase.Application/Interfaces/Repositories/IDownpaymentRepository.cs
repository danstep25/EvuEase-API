using EvuEase.Application.Common;
using EvuEase.Application.DTOs.Downpayment;
using EvuEase.Domain.Entities;

namespace EvuEase.Application.Interfaces.Repositories;

public interface IDownpaymentRepository
{
    Task<PagedResults<DpPercentage>> GetAllAsync(DownpaymentRequest request);
    Task<DpPercentage?> GetByIdAsync(long id);
    Task<DpPercentage> CreateAsync(DpPercentage entity);
    Task<DpPercentage> SupersedeWithNewRowAsync(DpPercentage oldRow, DpPercentage newEntity);
    Task DeleteAsync(DpPercentage entity);
    Task<bool> ExistsOtherActiveWithProgramCodeAsync(string programCode, long? excludeId);
    Task<IReadOnlyList<DpPercentage>> GetHistoryByProgramCodeAsync(string programCode);
}
