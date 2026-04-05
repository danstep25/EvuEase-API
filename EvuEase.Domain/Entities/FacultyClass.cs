namespace EvuEase.Domain.Entities;

public class FacultyClass : BaseEntity
{
    public long id { get; private set; }

    public string course_code { get; private set; } = string.Empty;
    public string class_number { get; private set; } = string.Empty;
    public string section { get; private set; } = string.Empty;
    public string course_title { get; private set; } = string.Empty;
    public string component { get; private set; } = string.Empty;
    public string academic_term { get; private set; } = string.Empty;
    public int enrolled_count { get; private set; }
    public string program_code { get; private set; } = string.Empty;
    public string year_level { get; private set; } = string.Empty;

    private FacultyClass() { }

    public static FacultyClass Create(
        string courseCode,
        string classNumber,
        string section,
        string courseTitle,
        string component,
        string academicTerm,
        int enrolledCount,
        string programCode,
        string yearLevel)
    {
        var e = new FacultyClass();
        var t = typeof(FacultyClass);
        t.GetProperty(nameof(course_code))?.SetValue(e, courseCode);
        t.GetProperty(nameof(class_number))?.SetValue(e, classNumber);
        t.GetProperty(nameof(section))?.SetValue(e, section);
        t.GetProperty(nameof(course_title))?.SetValue(e, courseTitle);
        t.GetProperty(nameof(component))?.SetValue(e, component);
        t.GetProperty(nameof(academic_term))?.SetValue(e, academicTerm);
        t.GetProperty(nameof(enrolled_count))?.SetValue(e, enrolledCount);
        t.GetProperty(nameof(program_code))?.SetValue(e, programCode);
        t.GetProperty(nameof(year_level))?.SetValue(e, yearLevel);
        t.GetProperty(nameof(status))?.SetValue(e, true);
        t.GetProperty(nameof(created_at))?.SetValue(e, DateTime.Now);
        return e;
    }

    public void Update(
        string courseCode,
        string classNumber,
        string section,
        string courseTitle,
        string component,
        string academicTerm,
        int enrolledCount,
        string programCode,
        string yearLevel)
    {
        var t = typeof(FacultyClass);
        t.GetProperty(nameof(course_code))?.SetValue(this, courseCode);
        t.GetProperty(nameof(class_number))?.SetValue(this, classNumber);
        t.GetProperty(nameof(section))?.SetValue(this, section);
        t.GetProperty(nameof(course_title))?.SetValue(this, courseTitle);
        t.GetProperty(nameof(component))?.SetValue(this, component);
        t.GetProperty(nameof(academic_term))?.SetValue(this, academicTerm);
        t.GetProperty(nameof(enrolled_count))?.SetValue(this, enrolledCount);
        t.GetProperty(nameof(program_code))?.SetValue(this, programCode);
        t.GetProperty(nameof(year_level))?.SetValue(this, yearLevel);
        t.GetProperty(nameof(updated_at))?.SetValue(this, DateTime.Now);
    }

    public void SetEnrolledCount(int enrolledCount)
    {
        if (enrolledCount < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(enrolledCount));
        }

        var t = typeof(FacultyClass);
        t.GetProperty(nameof(enrolled_count))?.SetValue(this, enrolledCount);
        t.GetProperty(nameof(updated_at))?.SetValue(this, DateTime.Now);
    }
}
