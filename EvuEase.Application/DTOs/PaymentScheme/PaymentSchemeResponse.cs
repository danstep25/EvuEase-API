namespace EvuEase.Application.DTOs.PaymentScheme;

public class PaymentSchemeResponse
{
    public long Id { get; set; }
    public string? SchoolYear { get; set; }
    public string? Semester { get; set; }
    public string? Description { get; set; }
    public int InstallmentCount { get; set; }
    public List<PaymentSchemeInstallmentDto> Installments { get; set; } = new();
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
