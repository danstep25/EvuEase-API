using EvuEase.Domain.Entities;

namespace EvuEase.Application.Interfaces.Repositories;

public interface IGradingSchemeBasisRepository
{
    Task<GradingSchemeBasis?> GetByAcademicTermKeyAsync(string academicTermKey, CancellationToken cancellationToken = default);

    Task<GradingSchemeBasis> CreateGradingSchemeBasisAsync(GradingSchemeBasis entity, CancellationToken cancellationToken = default);

    Task<GradingSchemeBasis> UpdateGradingSchemeBasisAsync(GradingSchemeBasis entity, CancellationToken cancellationToken = default);
}
