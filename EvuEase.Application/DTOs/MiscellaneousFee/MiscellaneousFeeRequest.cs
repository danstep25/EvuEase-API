using EvuEase.Application.DTOs;

namespace EvuEase.Application.DTOs.MiscellaneousFee;

public class MiscellaneousFeeRequest : FilterBaseDto
{
    public string? SyId { get; set; }
    public string? Batch { get; set; }
    public string? Semester { get; set; }
    public string? MiscellaneousFee { get; set; }
}



