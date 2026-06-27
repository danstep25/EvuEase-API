namespace EvuEase.Domain.Entities;

public class Student : BaseEntity
{
    public long id { get; private set; }
    public string student_number { get; private set; } = string.Empty;
    public string first_name { get; private set; } = string.Empty;
    public string last_name { get; private set; } = string.Empty;
    public string? middle_name { get; private set; }
    public string program_code { get; private set; } = string.Empty;
    public string program_title { get; private set; } = string.Empty;
    public string year_level { get; private set; } = string.Empty;
    public string student_type { get; private set; } = string.Empty;
    public string enrollment_status { get; private set; } = string.Empty;
    public string? curriculum_code { get; private set; }
    public string? address { get; private set; }
    public string? contact_number { get; private set; }
    public string? email { get; private set; }
    public string? gender { get; private set; }
    public DateOnly? birthdate { get; private set; }
    public string? portal_password_hash { get; private set; }

    private Student() { }

    public static Student Create(
        string studentNumber,
        string firstName,
        string lastName,
        string? middleName,
        string programCode,
        string programTitle,
        string yearLevel,
        string studentType,
        string enrollmentStatus,
        string? address = null,
        string? contactNumber = null,
        string? email = null,
        string? gender = null,
        DateOnly? birthdate = null)
    {
        var student = new Student();
        var type = typeof(Student);

        type.GetProperty(nameof(student_number))?.SetValue(student, studentNumber);
        type.GetProperty(nameof(first_name))?.SetValue(student, firstName);
        type.GetProperty(nameof(last_name))?.SetValue(student, lastName);
        type.GetProperty(nameof(middle_name))?.SetValue(student, middleName);
        type.GetProperty(nameof(program_code))?.SetValue(student, programCode);
        type.GetProperty(nameof(program_title))?.SetValue(student, programTitle);
        type.GetProperty(nameof(year_level))?.SetValue(student, yearLevel);
        type.GetProperty(nameof(student_type))?.SetValue(student, studentType);
        type.GetProperty(nameof(enrollment_status))?.SetValue(student, enrollmentStatus);
        type.GetProperty(nameof(Student.address))?.SetValue(student, address);
        type.GetProperty(nameof(Student.contact_number))?.SetValue(student, contactNumber);
        type.GetProperty(nameof(Student.email))?.SetValue(student, email);
        type.GetProperty(nameof(Student.gender))?.SetValue(student, gender);
        type.GetProperty(nameof(Student.birthdate))?.SetValue(student, birthdate);
        type.GetProperty(nameof(created_at))?.SetValue(student, DateTime.Now);

        return student;
    }

    public void Update(
        string studentNumber,
        string firstName,
        string lastName,
        string? middleName,
        string programCode,
        string programTitle,
        string yearLevel,
        string studentType,
        string enrollmentStatus,
        string? address = null,
        string? contactNumber = null,
        string? email = null,
        string? gender = null,
        DateOnly? birthdate = null)
    {
        var type = typeof(Student);

        type.GetProperty(nameof(this.student_number))?.SetValue(this, studentNumber);
        type.GetProperty(nameof(this.first_name))?.SetValue(this, firstName);
        type.GetProperty(nameof(this.last_name))?.SetValue(this, lastName);
        type.GetProperty(nameof(this.middle_name))?.SetValue(this, middleName);
        type.GetProperty(nameof(this.program_code))?.SetValue(this, programCode);
        type.GetProperty(nameof(this.program_title))?.SetValue(this, programTitle);
        type.GetProperty(nameof(this.year_level))?.SetValue(this, yearLevel);
        type.GetProperty(nameof(this.student_type))?.SetValue(this, studentType);
        type.GetProperty(nameof(this.enrollment_status))?.SetValue(this, enrollmentStatus);
        type.GetProperty(nameof(Student.address))?.SetValue(this, address);
        type.GetProperty(nameof(Student.contact_number))?.SetValue(this, contactNumber);
        type.GetProperty(nameof(Student.email))?.SetValue(this, email);
        type.GetProperty(nameof(Student.gender))?.SetValue(this, gender);
        type.GetProperty(nameof(Student.birthdate))?.SetValue(this, birthdate);
        type.GetProperty(nameof(updated_at))?.SetValue(this, DateTime.Now);
    }

    public void SetCurriculumCode(string curriculumCode)
    {
        var type = typeof(Student);
        type.GetProperty(nameof(curriculum_code))?.SetValue(this, curriculumCode.Trim());
        type.GetProperty(nameof(updated_at))?.SetValue(this, DateTime.UtcNow);
    }

    public bool HasPortalAccess() => !string.IsNullOrWhiteSpace(portal_password_hash);

    public void SetPortalPasswordHash(string? passwordHash)
    {
        var type = typeof(Student);
        type.GetProperty(nameof(portal_password_hash))?.SetValue(this, passwordHash);
        type.GetProperty(nameof(updated_at))?.SetValue(this, DateTime.UtcNow);
    }
}
