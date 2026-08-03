using EvuEase.Application.Analytics;
using EvuEase.Application.DTOs.Analytics;

namespace EvuEase.Application.Interfaces.Repositories;

public interface IAnalyticsRepository
{
    Task<int> CountStudentsAsync(AnalyticsDashboardRequest request, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<AnalyticsCountLabelDto>> GetStudentsByProgramAsync(
        AnalyticsDashboardRequest request,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<AnalyticsCountLabelDto>> GetStudentsByYearLevelAsync(
        AnalyticsDashboardRequest request,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<EnrollmentAnalyticsRow>> GetEnrollmentRowsAsync(
        AnalyticsDashboardRequest request,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<(DateTime Timestamp, string Module, string Action)>> GetChargeSlipLogEventsAsync(
        AnalyticsDashboardRequest request,
        CancellationToken cancellationToken = default);
}
