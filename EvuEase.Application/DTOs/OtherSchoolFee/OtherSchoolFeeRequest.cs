using EvuEase.Application.DTOs;

namespace EvuEase.Application.DTOs.OtherSchoolFee;

public class OtherSchoolFeeRequest : FilterBaseDto
{
    public string? SyId { get; set; }
    public string? Batch { get; set; }
    public string? Semester { get; set; }
    public string? SchoolFee { get; set; }
}


