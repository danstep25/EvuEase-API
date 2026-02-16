using EvuEase.Application.DTOs;

namespace EvuEase.Application.DTOs.Curricula;

public class CurriculaRequest : FilterBaseDto
{
    public string? CurriculumCode { get; set; }
    public string? Version { get; set; }
    public long? ProgramId { get; set; }
    public long? SyId { get; set; }
    public string? Status { get; set; }
}

