namespace EvuEase.Application.DTOs.Course;

public sealed class CourseBatchImportRowDto
{
    public int RowNumber { get; set; }
    public string CourseCode { get; set; } = string.Empty;
    public string CourseTitle { get; set; } = string.Empty;
    public int CourseLecUnits { get; set; }
    public int CourseLabUnits { get; set; }
    public int CourseTotalUnits { get; set; }
    public string CourseYearLevel { get; set; } = string.Empty;
    public string CourseSemester { get; set; } = string.Empty;
    public string? Prerequisites { get; set; }
    public string? CourseComponent { get; set; }
    public bool Selected { get; set; } = true;
}
