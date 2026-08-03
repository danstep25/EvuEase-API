namespace EvuEase.Application.DTOs.Course;

public sealed class CourseBatchImportPreviewResponse
{
    public string CurriculumCode { get; set; } = string.Empty;
    public string ProgramCode { get; set; } = string.Empty;
    public int TotalRows { get; set; }
    public int ValidRows { get; set; }
    public int WarningRows { get; set; }
    public int ErrorRows { get; set; }
    public int SkippedPdfLines { get; set; }
    public List<string> ParseWarnings { get; set; } = new();
    public string? DetectedReferenceNumber { get; set; }
    public List<CourseBatchImportPreviewRowDto> Rows { get; set; } = new();
}
