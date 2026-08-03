namespace EvuEase.Application.DTOs.Course;

public sealed class CourseBatchPdfDetectionResponse
{
    public string? ReferenceNumber { get; set; }
    public string? ProgramCode { get; set; }
    public string? CurriculumCode { get; set; }
    public string? SchoolYear { get; set; }
    public long? ProgramId { get; set; }
    public bool CurriculumFound { get; set; }
    public string? Message { get; set; }
}
