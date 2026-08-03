using EvuEase.Application.DTOs;

namespace EvuEase.Application.DTOs.CreditRequest;

public class CreditRequestRequest : FilterBaseDto
{
    public string? RequestStatus { get; set; }
}
