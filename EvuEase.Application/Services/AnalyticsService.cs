using EvuEase.Application.Analytics;
using EvuEase.Application.DTOs.Analytics;
using EvuEase.Application.Interfaces.Repositories;
using EvuEase.Application.Interfaces.Services;

namespace EvuEase.Application.Services;

public class AnalyticsService : IAnalyticsService
{
    private readonly IAnalyticsRepository _analyticsRepository;

    public AnalyticsService(IAnalyticsRepository analyticsRepository)
    {
        _analyticsRepository = analyticsRepository;
    }

    public async Task<AnalyticsDashboardResponse> GetDashboardAsync(
        AnalyticsDashboardRequest request,
        CancellationToken cancellationToken = default)
    {
        var totalStudents = await _analyticsRepository.CountStudentsAsync(request, cancellationToken);
        var studentsByProgram = await _analyticsRepository.GetStudentsByProgramAsync(request, cancellationToken);
        var studentsByYearLevel = await _analyticsRepository.GetStudentsByYearLevelAsync(request, cancellationToken);
        var enrollments = await _analyticsRepository.GetEnrollmentRowsAsync(request, cancellationToken);
        var chargeSlipLogs = await _analyticsRepository.GetChargeSlipLogEventsAsync(request, cancellationToken);

        return AnalyticsDashboardComputer.Compute(
            totalStudents,
            enrollments,
            studentsByProgram,
            studentsByYearLevel,
            chargeSlipLogs);
    }
}
