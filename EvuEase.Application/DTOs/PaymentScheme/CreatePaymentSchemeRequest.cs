namespace EvuEase.Application.DTOs.PaymentScheme;

public class CreatePaymentSchemeRequest
{
    public string? SchoolYear { get; set; }
    public string? Semester { get; set; }
    public string? Description { get; set; }
    public List<PaymentSchemeInstallmentInputDto> Installments { get; set; } = new();
}
