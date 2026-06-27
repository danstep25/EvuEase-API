using EvuEase.Application.DTOs;

namespace EvuEase.Application.DTOs.PaymentScheme;

public class PaymentSchemeRequest : FilterBaseDto
{
    public string? SchoolYear { get; set; }
    public string? Semester { get; set; }
}
