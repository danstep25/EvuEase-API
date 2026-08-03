namespace EvuEase.Application.DTOs.CreditRequest;

public class CreditRequestLineResponse
{
    public long Id { get; set; }
    public int SortOrder { get; set; }
    public string? AppliedCourseCode { get; set; }
    public string? AppliedCourseTitle { get; set; }
    public decimal AppliedLecUnits { get; set; }
    public decimal AppliedLabUnits { get; set; }
    public string? Grade { get; set; }
    public string? EquivalentCourseCode { get; set; }
    public string? EquivalentCourseTitle { get; set; }
    public decimal? EquivalentLecUnits { get; set; }
    public decimal? EquivalentLabUnits { get; set; }
    public decimal? EquivalentTotalUnits { get; set; }
}
