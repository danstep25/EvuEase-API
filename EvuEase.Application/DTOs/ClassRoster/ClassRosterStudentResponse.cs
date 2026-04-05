namespace EvuEase.Application.DTOs.ClassRoster;




public class ClassRosterStudentResponse
{
    
    public long Id { get; set; }

    public string StudentId { get; set; } = string.Empty;

    
    public string DisplayName { get; set; } = string.Empty;

    public string ProgramCode { get; set; } = string.Empty;

    public string YearLevel { get; set; } = string.Empty;

    
    public string? OfficialGrade { get; set; }

    
    public string? Remarks { get; set; }
}
