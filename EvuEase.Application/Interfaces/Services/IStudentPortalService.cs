using EvuEase.Application.DTOs.Student;
using EvuEase.Application.DTOs.StudentPortal;

namespace EvuEase.Application.Interfaces.Services;

public interface IStudentPortalService
{
    Task<StudentPortalAuthResponse> LoginAsync(StudentPortalLoginRequest request, CancellationToken cancellationToken = default);
    Task<StudentResponse?> GetProfileAsync(long studentId, CancellationToken cancellationToken = default);
    Task<StudentEnrollmentOverviewResponse?> GetEnrollmentsAsync(long studentId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<StudentPortalPendingSubjectDto>> GetPendingSubjectsAsync(long studentId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<StudentPortalGradeHistoryGroupDto>> GetGradeHistoryAsync(long studentId, CancellationToken cancellationToken = default);
    Task<StudentPortalDashboardResponse?> GetDashboardAsync(long studentId, CancellationToken cancellationToken = default);
    Task SetNewPasswordAsync(long studentId, StudentPortalSetNewPasswordRequest request, CancellationToken cancellationToken = default);
    Task RequestPasswordResetAsync(StudentPortalPasswordResetCreateRequest request, CancellationToken cancellationToken = default);
    Task RequestPasswordResetForStudentAsync(long studentId, string? reason, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<StudentPortalPasswordResetRequestResponse>> ListPasswordResetRequestsAsync(string? status, CancellationToken cancellationToken = default);
    Task<StudentPortalIssueTemporaryPasswordResponse> IssueTemporaryPasswordAsync(long requestId, StudentPortalIssueTemporaryPasswordRequest request, string? resolvedBy, CancellationToken cancellationToken = default);
    Task RejectPasswordResetRequestAsync(long requestId, StudentPortalPasswordResetRejectRequest request, string? resolvedBy, CancellationToken cancellationToken = default);
}
