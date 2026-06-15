namespace EvuEase.Application.DTOs.CreditRequest;

public class CreateCreditRequestRequest
{
    public string StudentNumber { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string? MiddleName { get; set; }
    public string LastName { get; set; } = string.Empty;
    public long ProgramId { get; set; }
    public long SyId { get; set; }
    public List<CreateCreditRequestLineRequest> Lines { get; set; } = new();
}
