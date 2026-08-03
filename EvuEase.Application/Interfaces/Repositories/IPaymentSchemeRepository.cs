using EvuEase.Application.Common;
using EvuEase.Application.DTOs.PaymentScheme;
using EvuEase.Domain.Entities;

namespace EvuEase.Application.Interfaces.Repositories;

public interface IPaymentSchemeRepository
{
    Task<PagedResults<PaymentScheme>> GetAllAsync(PaymentSchemeRequest request);
    Task<PaymentScheme?> GetByIdAsync(long id);
    Task<List<PaymentSchemeInstallment>> GetInstallmentsBySchemeIdsAsync(IEnumerable<long> schemeIds);
    Task<PaymentScheme> CreateAsync(PaymentScheme scheme, IEnumerable<PaymentSchemeInstallment> installments);
    Task<PaymentScheme> UpdateAsync(PaymentScheme scheme, IEnumerable<PaymentSchemeInstallment> installments);
    Task DeleteAsync(PaymentScheme scheme);
    Task<bool> ExistsOtherActiveAsync(string schoolYear, string semester, long? excludeId);
}
