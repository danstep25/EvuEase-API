namespace EvuEase.Domain.Entities;

public class GradingSchemeBasis : BaseEntity
{
    public long id { get; private set; }

    public string academic_term_key { get; private set; } = string.Empty;

    public string grading_scheme_code { get; private set; } = string.Empty;
    public string grading_scheme_description { get; private set; } = string.Empty;
    public string grading_basis_code { get; private set; } = string.Empty;
    public string grading_basis_description { get; private set; } = string.Empty;

    private GradingSchemeBasis() { }

    public static GradingSchemeBasis Create(
        string academicTermKey,
        string gradingSchemeCode,
        string gradingSchemeDescription,
        string gradingBasisCode,
        string gradingBasisDescription)
    {
        var entity = new GradingSchemeBasis();
        var type = typeof(GradingSchemeBasis);

        type.GetProperty(nameof(academic_term_key))?.SetValue(entity, academicTermKey);
        type.GetProperty(nameof(grading_scheme_code))?.SetValue(entity, gradingSchemeCode);
        type.GetProperty(nameof(grading_scheme_description))?.SetValue(entity, gradingSchemeDescription);
        type.GetProperty(nameof(grading_basis_code))?.SetValue(entity, gradingBasisCode);
        type.GetProperty(nameof(grading_basis_description))?.SetValue(entity, gradingBasisDescription);
        type.GetProperty(nameof(status))?.SetValue(entity, true);
        type.GetProperty(nameof(created_at))?.SetValue(entity, DateTime.Now);

        return entity;
    }

    public void Update(
        string gradingSchemeCode,
        string gradingSchemeDescription,
        string gradingBasisCode,
        string gradingBasisDescription)
    {
        var type = typeof(GradingSchemeBasis);

        type.GetProperty(nameof(grading_scheme_code))?.SetValue(this, gradingSchemeCode);
        type.GetProperty(nameof(grading_scheme_description))?.SetValue(this, gradingSchemeDescription);
        type.GetProperty(nameof(grading_basis_code))?.SetValue(this, gradingBasisCode);
        type.GetProperty(nameof(grading_basis_description))?.SetValue(this, gradingBasisDescription);
        type.GetProperty(nameof(updated_at))?.SetValue(this, DateTime.Now);
    }
}
