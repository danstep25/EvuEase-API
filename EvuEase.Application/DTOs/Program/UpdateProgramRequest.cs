namespace EvuEase.Application.DTOs.Program;

public class UpdateProgramRequest
{
    public long ProgramId { get; set; }
    public string ProgramCode { get; set; } = string.Empty;
    public string ProgramTitle { get; set; } = string.Empty;
    public int ProgramCompletionYears { get; set; }
    public int? ProgramTotalUnits { get; set; }
    public string ProgramStatus { get; set; } = "active";
}


