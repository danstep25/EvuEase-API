namespace EvuEase.Domain.Entities;

public class Course : BaseEntity
{
    public string course_code { get; private set; } = string.Empty;
    public int curriculum_id { get; private set; }
    public int program_id { get; private set; }
    public string course_title { get; private set; } = string.Empty;
    public int course_lec_units { get; private set; }
    public int course_lab_units { get; private set; }
    public int course_total_units { get; private set; }
    public string course_yearlevel { get; private set; } = string.Empty;
    public string course_semester { get; private set; } = string.Empty;
    public int course_has_prerequities { get; private set; }
}

