using EvuEase.Application.Common;
using EvuEase.Application.DTOs.PaymentScheme;

namespace EvuEase.Application.Interfaces.Services;

public interface IPaymentSchemeService
{
    Task<PagedResults<PaymentSchemeResponse>> GetAllAsync(PaymentSchemeRequest request);
    Task<PaymentSchemeResponse?> GetByIdAsync(long id);
    Task<PaymentSchemeResponse> CreateAsync(CreatePaymentSchemeRequest request);
    Task<PaymentSchemeResponse> UpdateAsync(UpdatePaymentSchemeRequest request);
    Task DeleteAsync(long id);
}
