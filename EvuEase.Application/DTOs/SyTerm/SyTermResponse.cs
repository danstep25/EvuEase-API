namespace EvuEase.Application.DTOs.SyTerm;

public class SyTermResponse
{
    public long SyId { get; set; }
    public string SyCode { get; set; } = string.Empty;
    public string SyYear { get; set; } = string.Empty;
    public string SySemester { get; set; } = string.Empty;
    public DateOnly SyStartDate { get; set; }
    public DateOnly SyEndDate { get; set; }
    public DateOnly SyEnrollmentStart { get; set; }
    public DateOnly SyEnrollmentEnd { get; set; }
    public string SyStatus { get; set; } = string.Empty;
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

