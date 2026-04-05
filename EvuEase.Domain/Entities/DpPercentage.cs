namespace EvuEase.Domain.Entities;

public class DpPercentage : BaseEntity
{
    public long id { get; private set; }
    public string? program_code { get; private set; }
    public string? program_title { get; private set; }
    public string? batch { get; private set; }
    public decimal downpayment_percent { get; private set; }
    public string? effective_school_year { get; private set; }
    public string? created_by { get; private set; }
    public string? updated_by { get; private set; }

    private DpPercentage()
    {
    }

    public static DpPercentage Create(
        string? programCode,
        string? programTitle,
        string? batch,
        decimal downpaymentPercent,
        string? effectiveSchoolYear)
    {
        var e = new DpPercentage();
        var t = typeof(DpPercentage);
        t.GetProperty(nameof(program_code))?.SetValue(e, programCode?.Trim());
        t.GetProperty(nameof(program_title))?.SetValue(e, programTitle?.Trim());
        t.GetProperty(nameof(batch))?.SetValue(e, batch?.Trim());
        t.GetProperty(nameof(downpayment_percent))?.SetValue(e, downpaymentPercent);
        t.GetProperty(nameof(effective_school_year))?.SetValue(e, effectiveSchoolYear?.Trim());
        t.GetProperty(nameof(status))?.SetValue(e, true);
        t.GetProperty(nameof(created_at))?.SetValue(e, DateTime.Now);
        return e;
    }

    public void SetCreatedBy(string? displayName)
    {
        var t = typeof(DpPercentage);
        var v = string.IsNullOrWhiteSpace(displayName) ? null : displayName.Trim();
        t.GetProperty(nameof(created_by))?.SetValue(this, v);
    }

    public void SetUpdatedBy(string? displayName)
    {
        var t = typeof(DpPercentage);
        var v = string.IsNullOrWhiteSpace(displayName) ? null : displayName.Trim();
        t.GetProperty(nameof(updated_by))?.SetValue(this, v);
    }

    public void Update(
        string? programCode,
        string? programTitle,
        string? batch,
        decimal downpaymentPercent,
        string? effectiveSchoolYear)
    {
        var t = typeof(DpPercentage);
        t.GetProperty(nameof(program_code))?.SetValue(this, programCode?.Trim());
        t.GetProperty(nameof(program_title))?.SetValue(this, programTitle?.Trim());
        t.GetProperty(nameof(batch))?.SetValue(this, batch?.Trim());
        t.GetProperty(nameof(downpayment_percent))?.SetValue(this, downpaymentPercent);
        t.GetProperty(nameof(effective_school_year))?.SetValue(this, effectiveSchoolYear?.Trim());
        t.GetProperty(nameof(updated_at))?.SetValue(this, DateTime.Now);
    }
}
