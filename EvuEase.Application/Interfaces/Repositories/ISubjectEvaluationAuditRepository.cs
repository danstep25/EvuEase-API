using EvuEase.Application.DTOs.EvaluationAudit;
using EvuEase.Domain.Entities;

namespace EvuEase.Application.Interfaces.Repositories;

public interface ISubjectEvaluationAuditRepository
{
    Task<IReadOnlyList<SubjectEvaluationAudit>> GetAllAsync(
        EvaluationAuditRequest request,
        CancellationToken cancellationToken = default);

    Task<int> CountAllAsync(EvaluationAuditRequest request, CancellationToken cancellationToken = default);

    Task<SubjectEvaluationAudit?> GetByIdAsync(long id, CancellationToken cancellationToken = default);

    Task<SubjectEvaluationAudit> CreateAsync(
        SubjectEvaluationAudit entity,
        CancellationToken cancellationToken = default);
}
