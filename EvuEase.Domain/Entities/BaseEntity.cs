namespace EvuEase.Domain.Entities;

public class BaseEntity
{
    public DateTime? created_at { get; set; }
    public DateTime? updated_at { get; set; }
    public bool status { get; set; } = true;

    
    public DateTime? deleted_at { get; set; }

    
    public string? deleted_by { get; set; }
}
