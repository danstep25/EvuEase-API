namespace EvuEase.Domain.Entities;

public class Program : BaseEntity
{
    public long program_id { get; private set; }
    public string program_code { get; private set; } = string.Empty;
    public string program_title { get; private set; } = string.Empty;
    public int program_completionyears { get; private set; }
    public int? program_totalunits { get; private set; }
    public string program_status { get; private set; } = string.Empty;
}
