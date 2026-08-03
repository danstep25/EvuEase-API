using EvuEase.Application.DTOs;

namespace EvuEase.Application.DTOs.Program;

public class ProgramRequest : FilterBaseDto
{
    public string? ProgramCode { get; set; }
    public string? ProgramTitle { get; set; }
    public string? Status { get; set; }
}

