using EvuEase.Application.DTOs.FacultyCenter;
using EvuEase.Application.Interfaces.Repositories;
using EvuEase.Application.Interfaces.Services;
using EvuEase.Domain.Entities;

namespace EvuEase.Application.Services;

public class ClassAssignmentService : IClassAssignmentService
{
    private readonly IGradingSchemeBasisRepository _repository;
    private readonly IGradeScaleRowRepository _gradeScaleRowRepository;

    public ClassAssignmentService(
        IGradingSchemeBasisRepository repository,
        IGradeScaleRowRepository gradeScaleRowRepository)
    {
        _repository = repository;
        _gradeScaleRowRepository = gradeScaleRowRepository;
    }

    public async Task<GradingSchemeBasisResponse?> GetGradingSchemeBasisAsync(string academicTermKey, CancellationToken cancellationToken = default)
    {
        var key = academicTermKey?.Trim() ?? string.Empty;
        if (string.IsNullOrEmpty(key))
        {
            return null;
        }

        var scheme = await _repository.GetByAcademicTermKeyAsync(key, cancellationToken);
        var gradeRows = await _gradeScaleRowRepository.GetByAcademicTermKeyAsync(key, cancellationToken);
        var rowDtos = gradeRows
            .Select(r => new GradeScaleRowResponse
            {
                Id = r.id,
                Mark = r.mark,
                Grade = r.grade,
                SortOrder = r.sort_order
            })
            .ToList();

        if (scheme == null)
        {
            return new GradingSchemeBasisResponse
            {
                AcademicTermKey = key,
                GradingSchemeCode = string.Empty,
                GradingSchemeDescription = string.Empty,
                GradingBasisCode = string.Empty,
                GradingBasisDescription = string.Empty,
                GradeScaleRows = rowDtos
            };
        }

        return new GradingSchemeBasisResponse
        {
            AcademicTermKey = scheme.academic_term_key,
            GradingSchemeCode = scheme.grading_scheme_code,
            GradingSchemeDescription = scheme.grading_scheme_description,
            GradingBasisCode = scheme.grading_basis_code,
            GradingBasisDescription = scheme.grading_basis_description,
            GradeScaleRows = rowDtos
        };
    }

    public async Task SaveGradingSchemeBasisAsync(SaveGradingSchemeBasisRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var termKey = request.AcademicTermKey?.Trim() ?? string.Empty;
        var schemeCode = request.GradingSchemeCode?.Trim() ?? string.Empty;
        var schemeDesc = request.GradingSchemeDescription?.Trim() ?? string.Empty;
        var basisCode = request.GradingBasisCode?.Trim() ?? string.Empty;
        var basisDesc = request.GradingBasisDescription?.Trim() ?? string.Empty;

        if (string.IsNullOrEmpty(termKey))
        {
            throw new ArgumentException("Academic term is required.", nameof(request.AcademicTermKey));
        }

        if (string.IsNullOrEmpty(schemeCode) || string.IsNullOrEmpty(schemeDesc) ||
            string.IsNullOrEmpty(basisCode) || string.IsNullOrEmpty(basisDesc))
        {
            throw new ArgumentException("Grading scheme and basis codes and descriptions are required.");
        }

        var gradeRequests = request.GradeScaleRows ?? new List<GradeScaleRowRequest>();
        foreach (var row in gradeRequests)
        {
            if (row.Mark < 0 || row.Grade < 0)
            {
                throw new ArgumentException("Each grade scale row must have non-negative mark and grade values.");
            }
        }

        var existing = await _repository.GetByAcademicTermKeyAsync(termKey, cancellationToken);

        if (existing == null)
        {
            var created = GradingSchemeBasis.Create(
                termKey,
                schemeCode,
                schemeDesc,
                basisCode,
                basisDesc);
            await _repository.CreateGradingSchemeBasisAsync(created, cancellationToken);
        }
        else
        {
            existing.Update(schemeCode, schemeDesc, basisCode, basisDesc);
            await _repository.UpdateGradingSchemeBasisAsync(existing, cancellationToken);
        }

        await _gradeScaleRowRepository.ReplaceForAcademicTermAsync(termKey, gradeRequests, cancellationToken);
    }
}
