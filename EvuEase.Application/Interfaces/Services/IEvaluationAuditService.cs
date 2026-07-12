using EvuEase.Application.Common;
using EvuEase.Application.DTOs.EvaluationAudit;

namespace EvuEase.Application.Interfaces.Services;

public interface IEvaluationAuditService
{
    Task<PagedResults<EvaluationAuditListItemResponse>> GetAllAsync(
        EvaluationAuditRequest request,
        CancellationToken cancellationToken = default);

    Task<EvaluationAuditDetailResponse?> GetByIdAsync(long id, CancellationToken cancellationToken = default);

    Task<EvaluationAuditDetailResponse> CreateAsync(
        CreateEvaluationAuditRequest request,
        string? evaluatedBy,
        CancellationToken cancellationToken = default);
}
