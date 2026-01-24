namespace EvuEase.Domain.Entities;

public class BaseEntity
{
    public DateTime? created_at { get; private set; }
    public DateTime? updated_at { get; private set; }
}
