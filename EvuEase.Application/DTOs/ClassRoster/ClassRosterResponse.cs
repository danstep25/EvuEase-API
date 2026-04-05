namespace EvuEase.Application.DTOs.ClassRoster;

public class ClassRosterResponse
{
    public long Id { get; set; }
    public string CourseCode { get; set; } = string.Empty;
    public string ClassNumber { get; set; } = string.Empty;
    public string Section { get; set; } = string.Empty;
    public string CourseTitle { get; set; } = string.Empty;
    public string Component { get; set; } = string.Empty;
    public string AcademicTerm { get; set; } = string.Empty;
    public int Enrolled { get; set; }
    public string ProgramCode { get; set; } = string.Empty;
    public string YearLevel { get; set; } = string.Empty;
}
