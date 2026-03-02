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

    private SyTerm() { }

    public static SyTerm Create(string syCode, string syYear, string sySemester, DateOnly syStartDate, DateOnly syEndDate, DateOnly syEnrollmentStart, DateOnly syEnrollmentEnd, string syStatus = "Inactive")
    {
        var syTerm = new SyTerm();
        var type = typeof(SyTerm);
        
        type.GetProperty(nameof(sy_code))?.SetValue(syTerm, syCode);
        type.GetProperty(nameof(sy_year))?.SetValue(syTerm, syYear);
        type.GetProperty(nameof(sy_semester))?.SetValue(syTerm, sySemester);
        type.GetProperty(nameof(sy_startdate))?.SetValue(syTerm, syStartDate);
        type.GetProperty(nameof(sy_enddate))?.SetValue(syTerm, syEndDate);
        type.GetProperty(nameof(sy_enrollmentstart))?.SetValue(syTerm, syEnrollmentStart);
        type.GetProperty(nameof(sy_enrollmentend))?.SetValue(syTerm, syEnrollmentEnd);
        type.GetProperty(nameof(sy_status))?.SetValue(syTerm, syStatus);
        type.GetProperty(nameof(created_at))?.SetValue(syTerm, DateTime.Now);
        
        return syTerm;
    }

    public void Update(string syCode, string syYear, string sySemester, DateOnly syStartDate, DateOnly syEndDate, DateOnly syEnrollmentStart, DateOnly syEnrollmentEnd, string syStatus)
    {
        var type = typeof(SyTerm);

        type.GetProperty(nameof(this.sy_code))?.SetValue(this, syCode);
        type.GetProperty(nameof(this.sy_year))?.SetValue(this, syYear);
        type.GetProperty(nameof(this.sy_semester))?.SetValue(this, sySemester);
        type.GetProperty(nameof(this.sy_startdate))?.SetValue(this, syStartDate);
        type.GetProperty(nameof(this.sy_enddate))?.SetValue(this, syEndDate);
        type.GetProperty(nameof(this.sy_enrollmentstart))?.SetValue(this, syEnrollmentStart);
        type.GetProperty(nameof(this.sy_enrollmentend))?.SetValue(this, syEnrollmentEnd);
        type.GetProperty(nameof(this.sy_status))?.SetValue(this, syStatus);
        type.GetProperty(nameof(updated_at))?.SetValue(this, DateTime.Now);
    }
}

