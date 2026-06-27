namespace EvuEase.Application.DTOs.PaymentScheme;

public class PaymentSchemeInstallmentDto
{
    public long Id { get; set; }
    public int InstallmentOrder { get; set; }
    public string? PaymentName { get; set; }
    public DateTime DueDate { get; set; }
}
