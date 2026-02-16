namespace EvuEase.Application.DTOs.Curricula;

public class CreateCurriculaRequest
{
    public string CurriculumCode { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public long ProgramId { get; set; }
    public long SyId { get; set; }
    public DateOnly EffectiveDate { get; set; }
    public string CurriculumStatus { get; set; } = "Inactive";
}

