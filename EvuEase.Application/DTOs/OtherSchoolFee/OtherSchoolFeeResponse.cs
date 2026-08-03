namespace EvuEase.Application.DTOs.OtherSchoolFee;

public class OtherSchoolFeeResponse
{
    public long Id { get; set; }
    public string? SyId { get; set; }
    public string? Batch { get; set; }
    public string? Semester { get; set; }
    public string? SchoolFee { get; set; }
    public decimal Cash { get; set; }
    public decimal LowMonthlyPayment { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}



