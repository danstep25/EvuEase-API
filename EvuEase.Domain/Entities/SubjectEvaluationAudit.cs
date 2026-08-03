namespace EvuEase.Domain.Entities;

public class SubjectEvaluationAudit : BaseEntity
{
    public long id { get; private set; }
    public long student_id { get; private set; }
    public string student_number { get; private set; } = string.Empty;
    public string student_name { get; private set; } = string.Empty;
    public string program_code { get; private set; } = string.Empty;
    public string program_year_level { get; private set; } = string.Empty;
    public string school_year { get; private set; } = string.Empty;
    public string semester { get; private set; } = string.Empty;
    public string school_year_term { get; private set; } = string.Empty;
    public int total_units_selected { get; private set; }
    public string evaluated_by { get; private set; } = string.Empty;
    public DateTime evaluated_at { get; private set; }
    public string evaluation_payload { get; private set; } = string.Empty;

    private SubjectEvaluationAudit() { }

    public static SubjectEvaluationAudit Create(
        long studentId,
        string studentNumber,
        string studentName,
        string programCode,
        string programYearLevel,
        string schoolYear,
        string semester,
        string schoolYearTerm,
        int totalUnitsSelected,
        string evaluatedBy,
        string evaluationPayloadJson)
    {
        var entity = new SubjectEvaluationAudit();
        var type = typeof(SubjectEvaluationAudit);
        var now = DateTime.UtcNow;

        type.GetProperty(nameof(student_id))?.SetValue(entity, studentId);
        type.GetProperty(nameof(student_number))?.SetValue(entity, studentNumber.Trim());
        type.GetProperty(nameof(student_name))?.SetValue(entity, studentName.Trim());
        type.GetProperty(nameof(program_code))?.SetValue(entity, programCode.Trim());
        type.GetProperty(nameof(program_year_level))?.SetValue(entity, programYearLevel.Trim());
        type.GetProperty(nameof(school_year))?.SetValue(entity, schoolYear.Trim());
        type.GetProperty(nameof(semester))?.SetValue(entity, semester.Trim());
        type.GetProperty(nameof(school_year_term))?.SetValue(entity, schoolYearTerm.Trim());
        type.GetProperty(nameof(total_units_selected))?.SetValue(entity, totalUnitsSelected);
        type.GetProperty(nameof(evaluated_by))?.SetValue(entity, evaluatedBy.Trim());
        type.GetProperty(nameof(evaluated_at))?.SetValue(entity, now);
        type.GetProperty(nameof(evaluation_payload))?.SetValue(entity, evaluationPayloadJson);
        type.GetProperty(nameof(status))?.SetValue(entity, true);
        type.GetProperty(nameof(created_at))?.SetValue(entity, now);

        return entity;
    }
}
