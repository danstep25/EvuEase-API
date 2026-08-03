using EvuEase.Application.DTOs.Analytics;

namespace EvuEase.Application.Interfaces.Services;

public interface IAnalyticsService
{
    Task<AnalyticsDashboardResponse> GetDashboardAsync(
        AnalyticsDashboardRequest request,
        CancellationToken cancellationToken = default);
}
