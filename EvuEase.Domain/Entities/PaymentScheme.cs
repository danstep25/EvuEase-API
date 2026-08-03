namespace EvuEase.Domain.Entities;

public class PaymentScheme : BaseEntity
{
    public long id { get; private set; }
    public string? school_year { get; private set; }
    public string? semester { get; private set; }
    public string? description { get; private set; }

    private PaymentScheme() { }

    public static PaymentScheme Create(string? schoolYear, string? semester, string? description)
    {
        var scheme = new PaymentScheme();
        var type = typeof(PaymentScheme);

        type.GetProperty(nameof(school_year))?.SetValue(scheme, schoolYear);
        type.GetProperty(nameof(semester))?.SetValue(scheme, semester);
        type.GetProperty(nameof(description))?.SetValue(scheme, description);
        type.GetProperty(nameof(status))?.SetValue(scheme, true);
        type.GetProperty(nameof(created_at))?.SetValue(scheme, DateTime.Now);

        return scheme;
    }

    public void Update(string? schoolYear, string? semester, string? description)
    {
        var type = typeof(PaymentScheme);

        type.GetProperty(nameof(school_year))?.SetValue(this, schoolYear);
        type.GetProperty(nameof(semester))?.SetValue(this, semester);
        type.GetProperty(nameof(description))?.SetValue(this, description);
        type.GetProperty(nameof(updated_at))?.SetValue(this, DateTime.Now);
    }
}
