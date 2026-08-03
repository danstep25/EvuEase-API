using EvuEase.Application.DTOs.Analytics;
using EvuEase.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EvuEase.API.Controller;

[Route("api/[controller]")]
public class AnalyticsController : BaseController
{
    private readonly IAnalyticsService _analyticsService;

    public AnalyticsController(IAnalyticsService analyticsService)
    {
        _analyticsService = analyticsService;
    }

    [Authorize]
    [HttpGet("dashboard")]
    public async Task<IActionResult> GetDashboard([FromQuery] AnalyticsDashboardRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var data = await _analyticsService.GetDashboardAsync(request, cancellationToken);
            return Ok(data);
        }
        catch (Exception ex)
        {
            return InternalServerError("An error occurred while retrieving analytics.", ex.Message);
        }
    }
}
