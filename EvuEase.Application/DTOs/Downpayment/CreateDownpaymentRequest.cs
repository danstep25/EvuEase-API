namespace EvuEase.Application.DTOs.Downpayment;

public class CreateDownpaymentRequest
{
    public string? ProgramCode { get; set; }
    public string? ProgramTitle { get; set; }
    public string? Batch { get; set; }
    public decimal DownpaymentPercent { get; set; }
    public string? EffectiveSchoolYear { get; set; }
}
