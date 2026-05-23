namespace EvuEase.Application.Analytics;

public sealed class EnrollmentAnalyticsRow
{
    public long StudentId { get; init; }
    public string ProgramCode { get; init; } = string.Empty;
    public string YearLevel { get; init; } = string.Empty;
    public string CourseCode { get; init; } = string.Empty;
    public string AcademicTerm { get; init; } = string.Empty;
    public int Units { get; init; }
    public string? OfficialGrade { get; init; }
    public string? StoredRemarks { get; init; }
    public DateTime? CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}
