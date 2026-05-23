namespace EvuEase.Application.DTOs.Student;

public class MigrateStudentCurriculumRequest
{
    public string CurriculumCode { get; set; } = string.Empty;
    public string? EffectiveSchoolYear { get; set; }
    public string? Reason { get; set; }
    public string? Notes { get; set; }
}
