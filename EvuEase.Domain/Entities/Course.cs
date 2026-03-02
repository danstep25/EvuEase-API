namespace EvuEase.Domain.Entities;

public class Course : BaseEntity
{
    public string course_code { get; private set; } = string.Empty;
    public long curriculum_id { get; private set; }
    public long program_id { get; private set; }
    public string course_title { get; private set; } = string.Empty;
    public int course_lec_units { get; private set; }
    public int course_lab_units { get; private set; }
    public int course_total_units { get; private set; }
    public string course_yearlevel { get; private set; } = string.Empty;
    public string course_semester { get; private set; } = string.Empty;
    public string? course_component { get; private set; }
    public string? prerequisites { get; private set; }
    public string? description { get; private set; }
    public int course_has_prerequities { get; private set; }

    private Course() { }

    public static Course Create(
        string courseCode,
        long curriculumId,
        long programId,
        string courseTitle,
        int courseTotalUnits,
        string courseYearLevel,
        string courseSemester,
        string? courseComponent = null,
        string? prerequisites = null,
        string? description = null)
    {
        var course = new Course();
        var type = typeof(Course);
        
        type.GetProperty(nameof(course_code))?.SetValue(course, courseCode);
        type.GetProperty(nameof(curriculum_id))?.SetValue(course, curriculumId);
        type.GetProperty(nameof(program_id))?.SetValue(course, programId);
        type.GetProperty(nameof(course_title))?.SetValue(course, courseTitle);
        type.GetProperty(nameof(course_lec_units))?.SetValue(course, 0);
        type.GetProperty(nameof(course_lab_units))?.SetValue(course, 0);
        type.GetProperty(nameof(course_total_units))?.SetValue(course, courseTotalUnits);
        type.GetProperty(nameof(course_yearlevel))?.SetValue(course, courseYearLevel);
        type.GetProperty(nameof(course_semester))?.SetValue(course, courseSemester);
        type.GetProperty(nameof(course_component))?.SetValue(course, courseComponent);
        type.GetProperty(nameof(prerequisites))?.SetValue(course, prerequisites);
        type.GetProperty(nameof(description))?.SetValue(course, description);
        type.GetProperty(nameof(course_has_prerequities))?.SetValue(course, string.IsNullOrWhiteSpace(prerequisites) ? 0 : 1);
        type.GetProperty(nameof(status))?.SetValue(course, true);
        type.GetProperty(nameof(created_at))?.SetValue(course, DateTime.Now);
        
        return course;
    }

    public void Update(
        long curriculumId,
        long programId,
        string courseTitle,
        int courseTotalUnits,
        string courseYearLevel,
        string courseSemester,
        string? courseComponent = null,
        string? prerequisites = null,
        string? description = null)
    {
        var type = typeof(Course);

        type.GetProperty(nameof(curriculum_id))?.SetValue(this, curriculumId);
        type.GetProperty(nameof(program_id))?.SetValue(this, programId);
        type.GetProperty(nameof(course_title))?.SetValue(this, courseTitle);
        type.GetProperty(nameof(course_total_units))?.SetValue(this, courseTotalUnits);
        type.GetProperty(nameof(course_yearlevel))?.SetValue(this, courseYearLevel);
        type.GetProperty(nameof(course_semester))?.SetValue(this, courseSemester);
        type.GetProperty(nameof(course_component))?.SetValue(this, courseComponent);
        type.GetProperty(nameof(prerequisites))?.SetValue(this, prerequisites);
        type.GetProperty(nameof(description))?.SetValue(this, description);
        type.GetProperty(nameof(course_has_prerequities))?.SetValue(this, string.IsNullOrWhiteSpace(prerequisites) ? 0 : 1);
        type.GetProperty(nameof(updated_at))?.SetValue(this, DateTime.Now);
    }
}

