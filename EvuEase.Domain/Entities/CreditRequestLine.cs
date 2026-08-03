namespace EvuEase.Domain.Entities;

public class CreditRequestLine : BaseEntity
{
    public long id { get; private set; }
    public long credit_request_id { get; private set; }
    public int sort_order { get; private set; }
    public string? applied_course_code { get; private set; }
    public string? applied_course_title { get; private set; }
    public decimal applied_lec_units { get; private set; }
    public decimal applied_lab_units { get; private set; }
    public string? grade { get; private set; }
    public string? equivalent_course_code { get; private set; }

    private CreditRequestLine() { }

    public static CreditRequestLine Create(
        long creditRequestId,
        int sortOrder,
        string? appliedCourseCode,
        string? appliedCourseTitle,
        decimal appliedLecUnits,
        decimal appliedLabUnits,
        string? grade,
        string? equivalentCourseCode)
    {
        var entity = new CreditRequestLine();
        var type = typeof(CreditRequestLine);

        type.GetProperty(nameof(credit_request_id))?.SetValue(entity, creditRequestId);
        type.GetProperty(nameof(sort_order))?.SetValue(entity, sortOrder);
        type.GetProperty(nameof(applied_course_code))?.SetValue(entity, appliedCourseCode);
        type.GetProperty(nameof(applied_course_title))?.SetValue(entity, appliedCourseTitle);
        type.GetProperty(nameof(applied_lec_units))?.SetValue(entity, appliedLecUnits);
        type.GetProperty(nameof(applied_lab_units))?.SetValue(entity, appliedLabUnits);
        type.GetProperty(nameof(grade))?.SetValue(entity, grade);
        type.GetProperty(nameof(equivalent_course_code))?.SetValue(entity, equivalentCourseCode);
        type.GetProperty(nameof(status))?.SetValue(entity, true);
        type.GetProperty(nameof(created_at))?.SetValue(entity, DateTime.Now);

        return entity;
    }
}
