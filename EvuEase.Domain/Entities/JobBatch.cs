namespace EvuEase.Domain.Entities;

public class JobBatch
{
    public string id { get; private set; } = string.Empty;
    public string name { get; private set; } = string.Empty;
    public int total_jobs { get; private set; }
    public int pending_jobs { get; private set; }
    public int failed_jobs { get; private set; }
    public string failed_job_ids { get; private set; } = string.Empty;
    public string? options { get; private set; }
    public int? cancelled_at { get; private set; }
    public int created_at { get; private set; }
    public int? finished_at { get; private set; }
}

