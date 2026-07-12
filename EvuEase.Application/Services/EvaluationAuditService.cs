using System.Text.Json;
using EvuEase.Application.Common;
using EvuEase.Application.DTOs.EvaluationAudit;
using EvuEase.Application.Interfaces.Repositories;
using EvuEase.Application.Interfaces.Services;
using EvuEase.Domain.Entities;

namespace EvuEase.Application.Services;

public class EvaluationAuditService : IEvaluationAuditService
{
    private readonly ISubjectEvaluationAuditRepository _repository;

    public EvaluationAuditService(ISubjectEvaluationAuditRepository repository)
    {
        _repository = repository;
    }

    public async Task<PagedResults<EvaluationAuditListItemResponse>> GetAllAsync(
        EvaluationAuditRequest request,
        CancellationToken cancellationToken = default)
    {
        var rows = await _repository.GetAllAsync(request, cancellationToken);
        var total = await _repository.CountAllAsync(request, cancellationToken);
        var responses = rows.Select(MapListItem).ToList();

        return new PagedResults<EvaluationAuditListItemResponse>(
            request.PageIndex,
            request.PageSize,
            total,
            total,
            responses);
    }

    public async Task<EvaluationAuditDetailResponse?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);
        return entity == null ? null : MapDetail(entity);
    }

    public async Task<EvaluationAuditDetailResponse> CreateAsync(
        CreateEvaluationAuditRequest request,
        string? evaluatedBy,
        CancellationToken cancellationToken = default)
    {
        if (request.StudentId <= 0)
        {
            throw new ArgumentException("Student ID is required.");
        }

        if (request.EvaluationData.ValueKind is JsonValueKind.Undefined or JsonValueKind.Null)
        {
            throw new ArgumentException("Evaluation data is required.");
        }

        var payloadJson = request.EvaluationData.GetRawText();
        var entity = SubjectEvaluationAudit.Create(
            request.StudentId,
            request.StudentNumber,
            request.StudentName,
            request.ProgramCode,
            request.ProgramYearLevel,
            request.SchoolYear,
            request.Semester,
            request.SchoolYearTerm,
            request.TotalUnitsSelected,
            string.IsNullOrWhiteSpace(evaluatedBy) ? "System" : evaluatedBy.Trim(),
            payloadJson);

        var created = await _repository.CreateAsync(entity, cancellationToken);
        return MapDetail(created);
    }

    private static EvaluationAuditListItemResponse MapListItem(SubjectEvaluationAudit entity)
    {
        return new EvaluationAuditListItemResponse
        {
            Id = entity.id,
            StudentId = entity.student_id,
            StudentNumber = entity.student_number,
            StudentName = entity.student_name,
            ProgramCode = entity.program_code,
            ProgramYearLevel = entity.program_year_level,
            SchoolYear = entity.school_year,
            Semester = entity.semester,
            SchoolYearTerm = entity.school_year_term,
            TotalUnitsSelected = entity.total_units_selected,
            EvaluatedBy = entity.evaluated_by,
            EvaluatedAt = entity.evaluated_at
        };
    }

    private static EvaluationAuditDetailResponse MapDetail(SubjectEvaluationAudit entity)
    {
        using var document = JsonDocument.Parse(entity.evaluation_payload);
        return new EvaluationAuditDetailResponse
        {
            Id = entity.id,
            StudentId = entity.student_id,
            StudentNumber = entity.student_number,
            StudentName = entity.student_name,
            ProgramCode = entity.program_code,
            ProgramYearLevel = entity.program_year_level,
            SchoolYear = entity.school_year,
            Semester = entity.semester,
            SchoolYearTerm = entity.school_year_term,
            TotalUnitsSelected = entity.total_units_selected,
            EvaluatedBy = entity.evaluated_by,
            EvaluatedAt = entity.evaluated_at,
            EvaluationData = document.RootElement.Clone()
        };
    }
}
