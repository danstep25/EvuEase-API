namespace EvuEase.Application.DTOs.TuitionFee;

public class TuitionFeeResponse
{
    public long Id { get; set; }
    public string? SyId { get; set; }
    public string? Batch { get; set; }
    public string? Semester { get; set; }
    public string? CourseCode { get; set; }
    public string? CourseTitle { get; set; }
    public string? Component { get; set; }
    public decimal? Units { get; set; }
    public decimal Cash { get; set; }
    public decimal LowMonthlyPayment { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}


