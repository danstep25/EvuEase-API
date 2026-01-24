namespace EvuEase.Domain.Entities;

public class SyTerm : BaseEntity
{
    public long sy_id { get; private set; }
    public string sy_code { get; private set; } = string.Empty;
    public string sy_year { get; private set; } = string.Empty;
    public string sy_semester { get; private set; } = string.Empty;
    public DateOnly sy_startdate { get; private set; }
    public DateOnly sy_enddate { get; private set; }
    public DateOnly sy_enrollmentstart { get; private set; }
    public DateOnly sy_enrollmentend { get; private set; }
    public string sy_status { get; private set; } = string.Empty;
}

