namespace EvuEase.Application.DTOs.CreditRequest;

public class CreditRequestResponse
{
    public long Id { get; set; }
    public string CreditRequestNo { get; set; } = string.Empty;
    public long? StudentId { get; set; }
    public string StudentNumber { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string? MiddleName { get; set; }
    public string LastName { get; set; } = string.Empty;
    public string StudentName { get; set; } = string.Empty;
    public long ProgramId { get; set; }
    public string ProgramCode { get; set; } = string.Empty;
    public string ProgramTitle { get; set; } = string.Empty;
    public long SyId { get; set; }
    public string SyCode { get; set; } = string.Empty;
    public string SyYear { get; set; } = string.Empty;
    public string SySemester { get; set; } = string.Empty;
    public string RequestStatus { get; set; } = string.Empty;
    public string? SignedPdfFileName { get; set; }
    public bool HasSignedPdf { get; set; }
    public List<CreditRequestLineResponse> Lines { get; set; } = new();
}
