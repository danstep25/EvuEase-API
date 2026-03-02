using EvuEase.Application.DTOs;

namespace EvuEase.Application.DTOs.TuitionFee;

public class TuitionFeeRequest : FilterBaseDto
{
    public string? SyId { get; set; }
    public string? Batch { get; set; }
    public string? Semester { get; set; }
    public string? CourseCode { get; set; }
    public string? CourseTitle { get; set; }
}


