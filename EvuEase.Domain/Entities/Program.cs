namespace EvuEase.Domain.Entities;

public class Program : BaseEntity
{
    public long program_id { get; private set; }
    public string program_code { get; private set; } = string.Empty;
    public string program_title { get; private set; } = string.Empty;
    public int program_completionyears { get; private set; }
    public int? program_totalunits { get; private set; }
    public string program_status { get; private set; } = string.Empty;

    // Private constructor for EF Core
    private Program() { }

    // Factory method for creating new programs
    public static Program Create(string programCode, string programTitle, int completionYears, int? totalUnits = null, string status = "active")
    {
        var program = new Program();
        var type = typeof(Program);
        
        type.GetProperty(nameof(program_code))?.SetValue(program, programCode);
        type.GetProperty(nameof(program_title))?.SetValue(program, programTitle);
        type.GetProperty(nameof(program_completionyears))?.SetValue(program, completionYears);
        type.GetProperty(nameof(program_totalunits))?.SetValue(program, totalUnits);
        type.GetProperty(nameof(program_status))?.SetValue(program, status);
        type.GetProperty(nameof(created_at))?.SetValue(program, DateTime.Now);
        
        return program;
    }

    public void Update(string programCode, string programTitle, int completionYears, int? totalUnits = null, string status = "active")
    {
        var type = typeof(Program);

        type.GetProperty(nameof(this.program_code))?.SetValue(this, programCode);
        type.GetProperty(nameof(this.program_title))?.SetValue(this, programTitle);
        type.GetProperty(nameof(this.program_completionyears))?.SetValue(this, completionYears);
        type.GetProperty(nameof(this.program_totalunits))?.SetValue(this, totalUnits);
        type.GetProperty(nameof(this.program_status))?.SetValue(this, status);
        type.GetProperty(nameof(updated_at))?.SetValue(this, DateTime.Now);
    }
}
