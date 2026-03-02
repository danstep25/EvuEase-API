namespace EvuEase.Application.DTOs.Course;

public class CourseResponse
{
    public string CourseCode { get; set; } = string.Empty;
    public string CurriculumCode { get; set; } = string.Empty;
    public long ProgramId { get; set; }
    public string ProgramCode { get; set; } = string.Empty;
    public string ProgramTitle { get; set; } = string.Empty;
    public string CourseTitle { get; set; } = string.Empty;
    public int CourseLecUnits { get; set; }
    public int CourseLabUnits { get; set; }
    public int CourseTotalUnits { get; set; }
    public string CourseYearLevel { get; set; } = string.Empty;
    public string CourseSemester { get; set; } = string.Empty;
    public string? CourseComponent { get; set; }
    public string? Prerequisites { get; set; }
    public string? Description { get; set; }
    public int CourseHasPrerequisites { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}



