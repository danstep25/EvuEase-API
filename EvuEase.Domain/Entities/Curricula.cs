namespace EvuEase.Domain.Entities;

public class Curricula : BaseEntity
{
    public long id { get; private set; }
    public string curriculum_code { get; private set; } = string.Empty;
    public string version { get; private set; } = string.Empty;
    public long program_id { get; private set; }
    public long sy_id { get; private set; }
    public DateOnly effective_date { get; private set; }
    public string curriculum_status { get; private set; } = string.Empty;

    private Curricula() { }

    public static Curricula Create(string curriculumCode, string version, long programId, long syId, DateOnly effectiveDate, string curriculumStatus = "Inactive")
    {
        var curricula = new Curricula();
        var type = typeof(Curricula);
        
        type.GetProperty(nameof(curriculum_code))?.SetValue(curricula, curriculumCode);
        type.GetProperty(nameof(version))?.SetValue(curricula, version);
        type.GetProperty(nameof(program_id))?.SetValue(curricula, programId);
        type.GetProperty(nameof(sy_id))?.SetValue(curricula, syId);
        type.GetProperty(nameof(effective_date))?.SetValue(curricula, effectiveDate);
        type.GetProperty(nameof(curriculum_status))?.SetValue(curricula, curriculumStatus);
        type.GetProperty(nameof(status))?.SetValue(curricula, true);
        type.GetProperty(nameof(created_at))?.SetValue(curricula, DateTime.Now);
        
        return curricula;
    }

    public void Update(string curriculumCode, string version, long programId, long syId, DateOnly effectiveDate, string curriculumStatus)
    {
        var type = typeof(Curricula);

        type.GetProperty(nameof(this.curriculum_code))?.SetValue(this, curriculumCode);
        type.GetProperty(nameof(this.version))?.SetValue(this, version);
        type.GetProperty(nameof(this.program_id))?.SetValue(this, programId);
        type.GetProperty(nameof(this.sy_id))?.SetValue(this, syId);
        type.GetProperty(nameof(this.effective_date))?.SetValue(this, effectiveDate);
        type.GetProperty(nameof(this.curriculum_status))?.SetValue(this, curriculumStatus);
        type.GetProperty(nameof(updated_at))?.SetValue(this, DateTime.Now);
    }
}

