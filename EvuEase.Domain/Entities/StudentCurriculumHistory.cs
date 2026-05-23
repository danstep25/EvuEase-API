namespace EvuEase.Domain.Entities;

public class StudentCurriculumHistory
{
    public long id { get; private set; }
    public long student_id { get; private set; }
    public string curriculum_code { get; private set; } = string.Empty;
    public string? effective_school_year { get; private set; }
    public string? reason { get; private set; }
    public string? notes { get; private set; }
    public string? migrated_by { get; private set; }
    public DateTime created_at { get; private set; }

    private StudentCurriculumHistory() { }

    public static StudentCurriculumHistory Create(
        long studentId,
        string curriculumCode,
        string? effectiveSchoolYear,
        string? reason,
        string? notes,
        string? migratedBy)
    {
        var row = new StudentCurriculumHistory();
        var type = typeof(StudentCurriculumHistory);

        type.GetProperty(nameof(student_id))?.SetValue(row, studentId);
        type.GetProperty(nameof(curriculum_code))?.SetValue(row, curriculumCode.Trim());
        type.GetProperty(nameof(effective_school_year))?.SetValue(row, effectiveSchoolYear?.Trim());
        type.GetProperty(nameof(reason))?.SetValue(row, reason?.Trim());
        type.GetProperty(nameof(notes))?.SetValue(row, notes?.Trim());
        type.GetProperty(nameof(migrated_by))?.SetValue(row, migratedBy?.Trim());
        type.GetProperty(nameof(created_at))?.SetValue(row, DateTime.UtcNow);

        return row;
    }
}
