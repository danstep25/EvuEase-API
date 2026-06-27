namespace EvuEase.Domain.Entities;

public class StudentPortalPasswordResetRequest
{
    public long id { get; private set; }
    public long student_id { get; private set; }
    public string student_number { get; private set; } = string.Empty;
    public string? reason { get; private set; }
    public string status { get; private set; } = "Pending";
    public string? registrar_notes { get; private set; }
    public string? resolved_by { get; private set; }
    public DateTime requested_at { get; private set; }
    public DateTime? resolved_at { get; private set; }

    private StudentPortalPasswordResetRequest() { }

    public static StudentPortalPasswordResetRequest Create(long studentId, string studentNumber, string? reason)
    {
        var row = new StudentPortalPasswordResetRequest();
        var type = typeof(StudentPortalPasswordResetRequest);

        type.GetProperty(nameof(student_id))?.SetValue(row, studentId);
        type.GetProperty(nameof(student_number))?.SetValue(row, studentNumber.Trim());
        type.GetProperty(nameof(reason))?.SetValue(row, string.IsNullOrWhiteSpace(reason) ? null : reason.Trim());
        type.GetProperty(nameof(status))?.SetValue(row, "Pending");
        type.GetProperty(nameof(requested_at))?.SetValue(row, DateTime.UtcNow);

        return row;
    }

    public void Resolve(string? registrarNotes, string? resolvedBy)
    {
        SetResolution("Resolved", registrarNotes, resolvedBy);
    }

    public void Reject(string? registrarNotes, string? resolvedBy)
    {
        SetResolution("Rejected", registrarNotes, resolvedBy);
    }

    private void SetResolution(string status, string? registrarNotes, string? resolvedBy)
    {
        var type = typeof(StudentPortalPasswordResetRequest);
        type.GetProperty(nameof(status))?.SetValue(this, status);
        type.GetProperty(nameof(registrar_notes))?.SetValue(this, string.IsNullOrWhiteSpace(registrarNotes) ? null : registrarNotes.Trim());
        type.GetProperty(nameof(resolved_by))?.SetValue(this, string.IsNullOrWhiteSpace(resolvedBy) ? null : resolvedBy.Trim());
        type.GetProperty(nameof(resolved_at))?.SetValue(this, DateTime.UtcNow);
    }
}
