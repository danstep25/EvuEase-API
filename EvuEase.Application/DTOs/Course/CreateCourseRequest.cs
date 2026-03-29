namespace EvuEase.Application.DTOs.Course;

public class CreateCourseRequest
{
    public string CourseCode { get; set; } = string.Empty;
    public string CurriculumCode { get; set; } = string.Empty;
    public long ProgramId { get; set; }
    public string CourseTitle { get; set; } = string.Empty;
    public int CourseTotalUnits { get; set; }
    public string CourseYearLevel { get; set; } = string.Empty;
    public string CourseSemester { get; set; } = string.Empty;
    public string? CourseComponent { get; set; }
    public string? Prerequisites { get; set; }
    public string? Description { get; set; }
}




