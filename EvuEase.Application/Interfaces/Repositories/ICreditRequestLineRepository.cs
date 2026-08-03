using EvuEase.Application.DTOs.CreditRequest;
using EvuEase.Domain.Entities;

namespace EvuEase.Application.Interfaces.Repositories;

public interface ICreditRequestLineRepository
{
    Task<IReadOnlyList<CreditRequestLine>> GetByCreditRequestIdAsync(long creditRequestId, CancellationToken cancellationToken = default);

    Task ReplaceForCreditRequestAsync(
        long creditRequestId,
        IReadOnlyList<CreateCreditRequestLineRequest> lines,
        CancellationToken cancellationToken = default);
}
