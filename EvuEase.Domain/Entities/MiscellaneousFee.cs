namespace EvuEase.Domain.Entities;

public class MiscellaneousFee : BaseEntity
{
    public long id { get; private set; }
    public string? sy_id { get; private set; }
    public string? batch { get; private set; }
    public string? semester { get; private set; }
    public string? miscellaneous_fee { get; private set; }
    public decimal cash { get; private set; }
    public decimal low_monthly_payment { get; private set; }

    private MiscellaneousFee() { }

    public static MiscellaneousFee Create(
        string? syId,
        string? batch,
        string? semester,
        string? miscellaneousFee,
        decimal cash,
        decimal lowMonthlyPayment)
    {
        var fee = new MiscellaneousFee();
        var type = typeof(MiscellaneousFee);

        type.GetProperty(nameof(sy_id))?.SetValue(fee, syId);
        type.GetProperty(nameof(batch))?.SetValue(fee, batch);
        type.GetProperty(nameof(semester))?.SetValue(fee, semester);
        type.GetProperty(nameof(miscellaneous_fee))?.SetValue(fee, miscellaneousFee);
        type.GetProperty(nameof(cash))?.SetValue(fee, cash);
        type.GetProperty(nameof(low_monthly_payment))?.SetValue(fee, lowMonthlyPayment);
        type.GetProperty(nameof(status))?.SetValue(fee, true);
        type.GetProperty(nameof(created_at))?.SetValue(fee, DateTime.Now);

        return fee;
    }

    public void Update(
        string? syId,
        string? batch,
        string? semester,
        string? miscellaneousFee,
        decimal cash,
        decimal lowMonthlyPayment)
    {
        var type = typeof(MiscellaneousFee);

        type.GetProperty(nameof(sy_id))?.SetValue(this, syId);
        type.GetProperty(nameof(batch))?.SetValue(this, batch);
        type.GetProperty(nameof(semester))?.SetValue(this, semester);
        type.GetProperty(nameof(miscellaneous_fee))?.SetValue(this, miscellaneousFee);
        type.GetProperty(nameof(cash))?.SetValue(this, cash);
        type.GetProperty(nameof(low_monthly_payment))?.SetValue(this, lowMonthlyPayment);
        type.GetProperty(nameof(updated_at))?.SetValue(this, DateTime.Now);
    }
}

