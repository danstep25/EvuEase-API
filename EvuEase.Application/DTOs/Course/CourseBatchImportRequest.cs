namespace EvuEase.Application.DTOs.Course;

public sealed class CourseBatchImportRequest
{
    public long ProgramId { get; set; }
    public string CurriculumCode { get; set; } = string.Empty;
    public List<CourseBatchImportRowDto> Rows { get; set; } = new();
}
