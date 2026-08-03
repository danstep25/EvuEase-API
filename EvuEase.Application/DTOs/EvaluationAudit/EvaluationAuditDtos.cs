using EvuEase.Application.DTOs;
using System.Text.Json;

namespace EvuEase.Application.DTOs.EvaluationAudit;

public class EvaluationAuditRequest : FilterBaseDto
{
}

public class CreateEvaluationAuditRequest
{
    public long StudentId { get; set; }
    public string StudentNumber { get; set; } = string.Empty;
    public string StudentName { get; set; } = string.Empty;
    public string ProgramCode { get; set; } = string.Empty;
    public string ProgramYearLevel { get; set; } = string.Empty;
    public string SchoolYear { get; set; } = string.Empty;
    public string Semester { get; set; } = string.Empty;
    public string SchoolYearTerm { get; set; } = string.Empty;
    public int TotalUnitsSelected { get; set; }
    public JsonElement EvaluationData { get; set; }
}

public class EvaluationAuditListItemResponse
{
    public long Id { get; set; }
    public long StudentId { get; set; }
    public string StudentNumber { get; set; } = string.Empty;
    public string StudentName { get; set; } = string.Empty;
    public string ProgramCode { get; set; } = string.Empty;
    public string ProgramYearLevel { get; set; } = string.Empty;
    public string SchoolYear { get; set; } = string.Empty;
    public string Semester { get; set; } = string.Empty;
    public string SchoolYearTerm { get; set; } = string.Empty;
    public int TotalUnitsSelected { get; set; }
    public string EvaluatedBy { get; set; } = string.Empty;
    public DateTime EvaluatedAt { get; set; }
}

public class EvaluationAuditDetailResponse : EvaluationAuditListItemResponse
{
    public JsonElement EvaluationData { get; set; }
}
