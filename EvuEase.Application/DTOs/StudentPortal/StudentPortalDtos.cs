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
}

public class StudentPortalChangePasswordRequest
{
    public string CurrentPassword { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
}

public class StudentPortalPasswordResetCreateRequest
{
    public string StudentNumber { get; set; } = string.Empty;
    public string? Reason { get; set; }
}

public class StudentPortalPasswordResetResolveRequest
{
    public string NewPortalPassword { get; set; } = string.Empty;
    public string? RegistrarNotes { get; set; }
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
