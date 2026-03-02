using EvuEase.Application.DTOs;

namespace EvuEase.Application.DTOs.Course;

public class CourseRequest : FilterBaseDto
{
    public string? CourseCode { get; set; }
    public string? CurriculumCode { get; set; }
    public long? ProgramId { get; set; }
    public string? CourseTitle { get; set; }
    public string? YearLevel { get; set; }
    public string? Semester { get; set; }
    public string? Status { get; set; }
}



