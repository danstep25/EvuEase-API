namespace EvuEase.Application.DTOs.Curricula;

public class CurriculaResponse
{
    public long Id { get; set; }
    public string CurriculumCode { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public long ProgramId { get; set; }
    public string ProgramCode { get; set; } = string.Empty;
    public string ProgramTitle { get; set; } = string.Empty;
    public long SyId { get; set; }
    public string SyYear { get; set; } = string.Empty;
    public DateOnly EffectiveDate { get; set; }
    public string CurriculumStatus { get; set; } = string.Empty;
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}



