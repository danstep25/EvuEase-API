namespace EvuEase.Domain.Entities;





public class FacultyClassEnrollment : BaseEntity
{
    public long id { get; private set; }
    public long faculty_class_id { get; private set; }
    public long student_id { get; private set; }

    
    public string? official_grade { get; private set; }

    
    public string? remarks { get; private set; }

    private FacultyClassEnrollment() { }

    public static FacultyClassEnrollment Create(long facultyClassId, long studentId)
    {
        var e = new FacultyClassEnrollment();
        var t = typeof(FacultyClassEnrollment);
        t.GetProperty(nameof(faculty_class_id))?.SetValue(e, facultyClassId);
        t.GetProperty(nameof(student_id))?.SetValue(e, studentId);
        t.GetProperty(nameof(status))?.SetValue(e, true);
        t.GetProperty(nameof(created_at))?.SetValue(e, DateTime.Now);
        return e;
    }

    
    
    
    
    public void SetOfficialGrade(string? officialGrade, string? storedRemarks = null)
    {
        var t = typeof(FacultyClassEnrollment);
        var normalized = string.IsNullOrWhiteSpace(officialGrade) ? null : officialGrade.Trim();
        t.GetProperty(nameof(official_grade))?.SetValue(this, normalized);
        var r = string.IsNullOrWhiteSpace(storedRemarks) ? null : storedRemarks.Trim();
        t.GetProperty(nameof(remarks))?.SetValue(this, r);
        t.GetProperty(nameof(updated_at))?.SetValue(this, DateTime.Now);
    }
}

