namespace EvuEase.Domain.Entities;

public class StudentPortalPasswordResetRequest
{
    public const string StatusPending = "Pending";
    public const string StatusTempIssued = "TempIssued";
    public const string StatusResolved = "Resolved";
    public const string StatusRejected = "Rejected";

    public long id { get; private set; }
    public long student_id { get; private set; }
    public string student_number { get; private set; } = string.Empty;
    public string? reason { get; private set; }
    public string status { get; private set; } = StatusPending;
    public string? registrar_notes { get; private set; }
    public string? resolved_by { get; private set; }
    public string? temporary_password { get; private set; }
    public DateTime? temporary_password_expires_at { get; private set; }
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
        type.GetProperty(nameof(status))?.SetValue(row, StatusPending);
        type.GetProperty(nameof(requested_at))?.SetValue(row, DateTime.UtcNow);

        return row;
    }

    /// <summary>
    /// Records the admin-issued temporary password. The request stays active
    /// (status <see cref="StatusTempIssued"/>) so the admin can view the
    /// temporary password until the student changes it.
    /// </summary>
    public void IssueTemporaryPassword(
        string temporaryPassword,
        DateTime expiresAtUtc,
        string? registrarNotes,
        string? issuedBy)
    {
        var type = typeof(StudentPortalPasswordResetRequest);
        type.GetProperty(nameof(status))?.SetValue(this, StatusTempIssued);
        type.GetProperty(nameof(temporary_password))?.SetValue(this, temporaryPassword);
        type.GetProperty(nameof(temporary_password_expires_at))?.SetValue(this, expiresAtUtc);
        type.GetProperty(nameof(registrar_notes))?.SetValue(this, string.IsNullOrWhiteSpace(registrarNotes) ? null : registrarNotes.Trim());
        type.GetProperty(nameof(resolved_by))?.SetValue(this, string.IsNullOrWhiteSpace(issuedBy) ? null : issuedBy.Trim());
    }

    /// <summary>
    /// Marks the request resolved once the student has set their own password.
    /// The stored temporary password is cleared.
    /// </summary>
    public void MarkResolved()
    {
        var type = typeof(StudentPortalPasswordResetRequest);
        type.GetProperty(nameof(status))?.SetValue(this, StatusResolved);
        type.GetProperty(nameof(temporary_password))?.SetValue(this, (string?)null);
        type.GetProperty(nameof(temporary_password_expires_at))?.SetValue(this, (DateTime?)null);
        type.GetProperty(nameof(resolved_at))?.SetValue(this, DateTime.UtcNow);
    }

    public void Reject(string? registrarNotes, string? resolvedBy)
    {
        var type = typeof(StudentPortalPasswordResetRequest);
        type.GetProperty(nameof(status))?.SetValue(this, StatusRejected);
        type.GetProperty(nameof(temporary_password))?.SetValue(this, (string?)null);
        type.GetProperty(nameof(temporary_password_expires_at))?.SetValue(this, (DateTime?)null);
        type.GetProperty(nameof(registrar_notes))?.SetValue(this, string.IsNullOrWhiteSpace(registrarNotes) ? null : registrarNotes.Trim());
        type.GetProperty(nameof(resolved_by))?.SetValue(this, string.IsNullOrWhiteSpace(resolvedBy) ? null : resolvedBy.Trim());
        type.GetProperty(nameof(resolved_at))?.SetValue(this, DateTime.UtcNow);
    }
}
