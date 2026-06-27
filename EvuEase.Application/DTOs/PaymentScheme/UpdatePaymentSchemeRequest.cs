namespace EvuEase.Application.DTOs.PaymentScheme;

public class UpdatePaymentSchemeRequest
{
    public long Id { get; set; }
    public string? SchoolYear { get; set; }
    public string? Semester { get; set; }
    public string? Description { get; set; }
    public List<PaymentSchemeInstallmentInputDto> Installments { get; set; } = new();
}
