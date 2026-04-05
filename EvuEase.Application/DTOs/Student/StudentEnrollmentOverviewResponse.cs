namespace EvuEase.Application.DTOs.Student;


public class StudentEnrollmentOverviewResponse
{
    public IReadOnlyList<StudentClassEnrollmentRowDto> Enrollments { get; set; } = Array.Empty<StudentClassEnrollmentRowDto>();
    public StudentAcademicSummaryDto Summary { get; set; } = new();
}
