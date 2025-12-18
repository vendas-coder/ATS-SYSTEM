using ATS.Application.DTOs.Applications;
using ATS.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ATS.API.Controllers;

[ApiController]
[Route("api/analytics")]
public class AnalyticsController : ControllerBase
{
    private readonly AnalyticsService _analyticsService;

    public AnalyticsController(AnalyticsService analyticsService)
    {
        _analyticsService = analyticsService;
    }

    [HttpGet("funnel")]
    [Authorize(Policy = "RecruiterOrAdmin")]
    public async Task<IActionResult> GetHiringFunnel()
    {
        var analytics = await _analyticsService.GetHiringFunnelAsync();
        return Ok(analytics);
    }

    [HttpGet("time-to-hire")]
    [Authorize(Policy = "RecruiterOrAdmin")]
    public async Task<IActionResult> GetTimeToHireAnalytics()
    {
        var analytics = await _analyticsService.GetTimeToHireAnalyticsAsync();
        return Ok(analytics);
    }
}