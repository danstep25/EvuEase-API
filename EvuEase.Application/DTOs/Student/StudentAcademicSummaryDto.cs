namespace EvuEase.Application.DTOs.Student;

public class StudentAcademicSummaryDto
{
    public int TotalUnitsCompleted { get; set; }
    public decimal? CumulativeGpa { get; set; }
    public int FailedSubjects { get; set; }
    public int RetakenSubjects { get; set; }
}
