using EvuEase.Application.DTOs;

namespace EvuEase.Application.DTOs.SyTerm;

public class SyTermRequest : FilterBaseDto
{
    public string? SyCode { get; set; }
    public string? SyYear { get; set; }
    public string? Semester { get; set; }
    public string? Status { get; set; }
}

