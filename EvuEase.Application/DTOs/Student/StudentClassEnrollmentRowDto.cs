namespace EvuEase.Application.DTOs.Student;


public class StudentClassEnrollmentRowDto
{
    public long EnrollmentId { get; set; }
    public long FacultyClassId { get; set; }
    public string CourseCode { get; set; } = string.Empty;
    public string CourseTitle { get; set; } = string.Empty;
    public string ClassNumber { get; set; } = string.Empty;
    public string Section { get; set; } = string.Empty;
    public string Component { get; set; } = string.Empty;
    
    public string AcademicTerm { get; set; } = string.Empty;
    public string ProgramCode { get; set; } = string.Empty;
    public string YearLevel { get; set; } = string.Empty;
    public int Units { get; set; }
    public string? OfficialGrade { get; set; }
    
    public string? Remarks { get; set; }
}
