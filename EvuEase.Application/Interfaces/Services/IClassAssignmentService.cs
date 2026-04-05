using EvuEase.Application.DTOs.FacultyCenter;

namespace EvuEase.Application.Interfaces.Services;

public interface IClassAssignmentService
{
    Task<GradingSchemeBasisResponse?> GetGradingSchemeBasisAsync(string academicTermKey, CancellationToken cancellationToken = default);

    Task SaveGradingSchemeBasisAsync(SaveGradingSchemeBasisRequest request, CancellationToken cancellationToken = default);
}
