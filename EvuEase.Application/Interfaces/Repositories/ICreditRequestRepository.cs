using EvuEase.Application.DTOs.CreditRequest;
using EvuEase.Domain.Entities;

namespace EvuEase.Application.Interfaces.Repositories;

public interface ICreditRequestRepository
{
    Task<IReadOnlyList<CreditRequest>> GetAllAsync(CreditRequestRequest request, CancellationToken cancellationToken = default);

    Task<int> CountAllAsync(CreditRequestRequest request, CancellationToken cancellationToken = default);

    Task<CreditRequest?> GetByIdAsync(long id, CancellationToken cancellationToken = default);

    Task<CreditRequest> CreateAsync(CreditRequest entity, CancellationToken cancellationToken = default);

    Task<CreditRequest> UpdateCreditRequestAsync(CreditRequest entity, CancellationToken cancellationToken = default);
}
