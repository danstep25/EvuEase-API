namespace EvuEase.Application.DTOs.Downpayment;

public class DownpaymentResponse
{
    public long Id { get; set; }
    public string? ProgramCode { get; set; }
    public string? ProgramTitle { get; set; }
    public string? Batch { get; set; }
    public decimal DownpaymentPercent { get; set; }
    public string? EffectiveSchoolYear { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
}
