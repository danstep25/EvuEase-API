namespace EvuEase.Application.DTOs.CreditRequest;

public class CreateCreditRequestLineRequest
{
    public string? AppliedCourseCode { get; set; }
    public string? AppliedCourseTitle { get; set; }
    public decimal AppliedLecUnits { get; set; }
    public decimal AppliedLabUnits { get; set; }
    public string? Grade { get; set; }
    public string? EquivalentCourseCode { get; set; }
}
