namespace EvuEase.Application.DTOs.SyTerm;

public class CreateSyTermRequest
{
    public string SyCode { get; set; } = string.Empty;
    public string SyYear { get; set; } = string.Empty;
    public string SySemester { get; set; } = string.Empty;
    public DateOnly SyStartDate { get; set; }
    public DateOnly SyEndDate { get; set; }
    public DateOnly SyEnrollmentStart { get; set; }
    public DateOnly SyEnrollmentEnd { get; set; }
    public string SyStatus { get; set; } = "Inactive";
}

