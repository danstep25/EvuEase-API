namespace EvuEase.Application.DTOs.Student;

public class CreateStudentRequest
{
    public string StudentNumber { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? MiddleName { get; set; }
    public string ProgramCode { get; set; } = string.Empty;
    public string ProgramTitle { get; set; } = string.Empty;
    public string YearLevel { get; set; } = string.Empty;
    public string StudentType { get; set; } = "Regular";
    public string EnrollmentStatus { get; set; } = "Active";
    public string? Address { get; set; }
    public string? ContactNumber { get; set; }
    public string? Email { get; set; }
    public string? Gender { get; set; }
    public DateOnly? Birthdate { get; set; }
    public string? PortalPassword { get; set; }
}
