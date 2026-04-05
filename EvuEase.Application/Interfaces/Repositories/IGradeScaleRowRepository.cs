using EvuEase.Application.DTOs.FacultyCenter;
using EvuEase.Domain.Entities;

namespace EvuEase.Application.Interfaces.Repositories;

public interface IGradeScaleRowRepository
{
    Task<IReadOnlyList<GradeScaleRow>> GetByAcademicTermKeyAsync(string academicTermKey, CancellationToken cancellationToken = default);

    Task ReplaceForAcademicTermAsync(string academicTermKey, IReadOnlyList<GradeScaleRowRequest> rows, CancellationToken cancellationToken = default);
}
