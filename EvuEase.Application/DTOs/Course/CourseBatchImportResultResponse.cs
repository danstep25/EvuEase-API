namespace EvuEase.Application.DTOs.Course;

public sealed class CourseBatchImportResultResponse
{
    public int ImportedCount { get; set; }
    public int SkippedCount { get; set; }
    public List<string> Errors { get; set; } = new();
    public List<string> Warnings { get; set; } = new();
}
