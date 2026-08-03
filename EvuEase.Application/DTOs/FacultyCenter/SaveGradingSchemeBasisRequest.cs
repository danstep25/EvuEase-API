namespace EvuEase.Application.DTOs.FacultyCenter;

public class SaveGradingSchemeBasisRequest
{
    public string AcademicTermKey { get; set; } = string.Empty;
    public string GradingSchemeCode { get; set; } = string.Empty;
    public string GradingSchemeDescription { get; set; } = string.Empty;
    public string GradingBasisCode { get; set; } = string.Empty;
    public string GradingBasisDescription { get; set; } = string.Empty;

    public List<GradeScaleRowRequest>? GradeScaleRows { get; set; }
}
