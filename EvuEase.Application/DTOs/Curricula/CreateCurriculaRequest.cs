namespace EvuEase.Application.DTOs.Curricula;

public class CreateCurriculaRequest
{
    public string Version { get; set; } = string.Empty;
    public long ProgramId { get; set; }
    public string ProgramCode { get; set; } = string.Empty;
    public long SyId { get; set; }
    public DateOnly EffectiveDate { get; set; }
    public string CurriculumStatus { get; set; } = "Inactive";
}



