namespace EvuEase.Application.DTOs.StudentPortal;

public class StudentPortalLoginRequest
{
    public string StudentNumber { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class StudentPortalAuthResponse
{
    public string Token { get; set; } = string.Empty;
    public long StudentId { get; set; }
    public string StudentNumber { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string ProgramCode { get; set; } = string.Empty;
    public string YearLevel { get; set; } = string.Empty;
    public string? CurriculumCode { get; set; }
    public string Role { get; set; } = "Student";
    public DateTime ExpiresAt { get; set; }

    /// <summary>
    /// True when the student signed in with an admin-issued temporary password
    /// and must set a new personal password before continuing.
    /// </summary>
    public bool MustChangePassword { get; set; }
}

/// <summary>
/// Sent from the forced "change temporary password" modal after a student logs
/// in with an admin-issued temporary password. Only a new password is required.
/// </summary>
public class StudentPortalSetNewPasswordRequest
{
    public string NewPassword { get; set; } = string.Empty;
}

public class StudentPortalPasswordResetCreateRequest
{
    public string StudentNumber { get; set; } = string.Empty;
    public string? Reason { get; set; }
}

/// <summary>
/// Password reset request raised by an already-authenticated student; the
/// student is identified from the JWT, so only an optional reason is needed.
/// </summary>
public class StudentPortalAuthenticatedResetRequest
{
    public string? Reason { get; set; }
}

/// <summary>
/// Admin action to issue an auto-generated temporary password for a request.
/// </summary>
public class StudentPortalIssueTemporaryPasswordRequest
{
    public string? RegistrarNotes { get; set; }
}

/// <summary>
/// Returned to the admin after issuing a temporary password, so it can be
/// relayed to the student. The plaintext temporary password is never stored.
/// </summary>
public class StudentPortalIssueTemporaryPasswordResponse
{
    public long RequestId { get; set; }
    public string StudentNumber { get; set; } = string.Empty;
    public string StudentName { get; set; } = string.Empty;
    public string TemporaryPassword { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
}

public class StudentPortalPasswordResetRejectRequest
{
    public string? RegistrarNotes { get; set; }
}

public class StudentPortalPasswordResetRequestResponse
{
    public long Id { get; set; }
    public long StudentId { get; set; }
    public string StudentNumber { get; set; } = string.Empty;
    public string StudentName { get; set; } = string.Empty;
    public string? Reason { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? RegistrarNotes { get; set; }
    public string? ResolvedBy { get; set; }
    public string? TemporaryPassword { get; set; }
    public DateTime? TemporaryPasswordExpiresAt { get; set; }
    public DateTime RequestedAt { get; set; }
    public DateTime? ResolvedAt { get; set; }
}

public class StudentPortalDashboardResponse
{
    public string StudentName { get; set; } = string.Empty;
    public string ProgramYearLevel { get; set; } = string.Empty;
    public string? CurriculumCode { get; set; }
    public string EnrollmentStatus { get; set; } = string.Empty;
    public int CurrentTermSubjects { get; set; }
    public int CompletedSubjects { get; set; }
    public int PendingSubjects { get; set; }
    public decimal? CumulativeGpa { get; set; }
    public int TotalUnitsCompleted { get; set; }
    public bool HasPendingPasswordReset { get; set; }
}

public class StudentPortalPendingSubjectDto
{
    public string CourseCode { get; set; } = string.Empty;
    public string SubjectDescription { get; set; } = string.Empty;
    public string Prerequisite { get; set; } = string.Empty;
    public int Units { get; set; }
    public string Component { get; set; } = string.Empty;
    public string YearTerm { get; set; } = string.Empty;
}

public class StudentPortalGradeHistoryRowDto
{
    public long EnrollmentId { get; set; }
    public string CourseCode { get; set; } = string.Empty;
    public string SubjectDescription { get; set; } = string.Empty;
    public int Units { get; set; }
    public string? Grade { get; set; }
    public string? Remarks { get; set; }
}

/// <summary>
/// Grade history grouped by the curriculum year level and semester of each
/// subject (e.g. "Year 1 - 1st Semester"), matching the Academic Records view.
/// </summary>
public class StudentPortalGradeHistoryGroupDto
{
    public string Label { get; set; } = string.Empty;
    public int SortYear { get; set; }
    public int SortSemester { get; set; }
    public List<StudentPortalGradeHistoryRowDto> Rows { get; set; } = new();
}
