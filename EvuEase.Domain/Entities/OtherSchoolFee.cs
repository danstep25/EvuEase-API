namespace EvuEase.Domain.Entities;

public class OtherSchoolFee : BaseEntity
{
    public long id { get; private set; }
    public string? sy_id { get; private set; }
    public string? batch { get; private set; }
    public string? semester { get; private set; }
    public string? school_fee { get; private set; }
    public decimal cash { get; private set; }
    public decimal low_monthly_payment { get; private set; }

    private OtherSchoolFee() { }

    public static OtherSchoolFee Create(
        string? syId,
        string? batch,
        string? semester,
        string? schoolFee,
        decimal cash,
        decimal lowMonthlyPayment)
    {
        var otherSchoolFee = new OtherSchoolFee();
        var type = typeof(OtherSchoolFee);

        type.GetProperty(nameof(sy_id))?.SetValue(otherSchoolFee, syId);
        type.GetProperty(nameof(batch))?.SetValue(otherSchoolFee, batch);
        type.GetProperty(nameof(semester))?.SetValue(otherSchoolFee, semester);
        type.GetProperty(nameof(school_fee))?.SetValue(otherSchoolFee, schoolFee);
        type.GetProperty(nameof(cash))?.SetValue(otherSchoolFee, cash);
        type.GetProperty(nameof(low_monthly_payment))?.SetValue(otherSchoolFee, lowMonthlyPayment);
        type.GetProperty(nameof(status))?.SetValue(otherSchoolFee, true);
        type.GetProperty(nameof(created_at))?.SetValue(otherSchoolFee, DateTime.Now);

        return otherSchoolFee;
    }

    public void Update(
        string? syId,
        string? batch,
        string? semester,
        string? schoolFee,
        decimal cash,
        decimal lowMonthlyPayment)
    {
        var type = typeof(OtherSchoolFee);

        type.GetProperty(nameof(sy_id))?.SetValue(this, syId);
        type.GetProperty(nameof(batch))?.SetValue(this, batch);
        type.GetProperty(nameof(semester))?.SetValue(this, semester);
        type.GetProperty(nameof(school_fee))?.SetValue(this, schoolFee);
        type.GetProperty(nameof(cash))?.SetValue(this, cash);
        type.GetProperty(nameof(low_monthly_payment))?.SetValue(this, lowMonthlyPayment);
        type.GetProperty(nameof(updated_at))?.SetValue(this, DateTime.Now);
    }
}

