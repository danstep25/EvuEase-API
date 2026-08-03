namespace EvuEase.Domain.Entities;

public class TuitionFee : BaseEntity
{
    public long id { get; private set; }
    public string? sy_id { get; private set; }
    public string? batch { get; private set; }
    public string? semester { get; private set; }
    public string? course_code { get; private set; }
    public string? course_title { get; private set; }
    public string? component { get; private set; }
    public decimal? units { get; private set; }
    public decimal cash { get; private set; }
    public decimal low_monthly_payment { get; private set; }

    private TuitionFee() { }

    public static TuitionFee Create(
        string? syId,
        string? batch,
        string? semester,
        string? courseCode,
        string? courseTitle,
        string? component,
        decimal? units,
        decimal cash,
        decimal lowMonthlyPayment)
    {
        var tuitionFee = new TuitionFee();
        var type = typeof(TuitionFee);
        
        type.GetProperty(nameof(sy_id))?.SetValue(tuitionFee, syId);
        type.GetProperty(nameof(batch))?.SetValue(tuitionFee, batch);
        type.GetProperty(nameof(semester))?.SetValue(tuitionFee, semester);
        type.GetProperty(nameof(course_code))?.SetValue(tuitionFee, courseCode);
        type.GetProperty(nameof(course_title))?.SetValue(tuitionFee, courseTitle);
        type.GetProperty(nameof(component))?.SetValue(tuitionFee, component);
        type.GetProperty(nameof(units))?.SetValue(tuitionFee, units);
        type.GetProperty(nameof(cash))?.SetValue(tuitionFee, cash);
        type.GetProperty(nameof(low_monthly_payment))?.SetValue(tuitionFee, lowMonthlyPayment);
        type.GetProperty(nameof(status))?.SetValue(tuitionFee, true);
        type.GetProperty(nameof(created_at))?.SetValue(tuitionFee, DateTime.Now);
        
        return tuitionFee;
    }

    public void Update(
        string? syId,
        string? batch,
        string? semester,
        string? courseCode,
        string? courseTitle,
        string? component,
        decimal? units,
        decimal cash,
        decimal lowMonthlyPayment)
    {
        var type = typeof(TuitionFee);

        type.GetProperty(nameof(sy_id))?.SetValue(this, syId);
        type.GetProperty(nameof(batch))?.SetValue(this, batch);
        type.GetProperty(nameof(semester))?.SetValue(this, semester);
        type.GetProperty(nameof(course_code))?.SetValue(this, courseCode);
        type.GetProperty(nameof(course_title))?.SetValue(this, courseTitle);
        type.GetProperty(nameof(component))?.SetValue(this, component);
        type.GetProperty(nameof(units))?.SetValue(this, units);
        type.GetProperty(nameof(cash))?.SetValue(this, cash);
        type.GetProperty(nameof(low_monthly_payment))?.SetValue(this, lowMonthlyPayment);
        type.GetProperty(nameof(updated_at))?.SetValue(this, DateTime.Now);
    }
}

