namespace EvuEase.Application.DTOs.FacultyCenter;

public class GradingSchemeBasisResponse
{
    public string AcademicTermKey { get; set; } = string.Empty;
    public string GradingSchemeCode { get; set; } = string.Empty;
    public string GradingSchemeDescription { get; set; } = string.Empty;
    public string GradingBasisCode { get; set; } = string.Empty;
    public string GradingBasisDescription { get; set; } = string.Empty;

    public List<GradeScaleRowResponse> GradeScaleRows { get; set; } = new();
}
