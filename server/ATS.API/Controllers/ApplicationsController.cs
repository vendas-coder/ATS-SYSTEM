using ATS.Application.Services;
using ATS.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ATS.Application.DTOs.Applications;




namespace ATS.API.Controllers;

[ApiController]
[Route("api/applications")]
public class ApplicationsController : ControllerBase
{
    private readonly ApplicationService _service;

    public ApplicationsController(ApplicationService service)
    {
        _service = service;
    }

    // Get all applications for a specific job
    [HttpGet("job/{jobId:guid}")]
    [Authorize(Policy = "RecruiterOrAdmin")]
public async Task<IActionResult> GetByJob(Guid jobId)
{
    var applications = await _service.GetByJobIdAsync(jobId);

    var response = applications.Select(a => new ApplicationResponse
    {
        Id = a.Id,
        CandidateId = a.CandidateId,
        JobId = a.JobId,
        Status = a.Status,
        AppliedAt = a.AppliedAt
    });

    return Ok(response);
}


    // Apply a candidate to a job
    [HttpPost]
    [Authorize(Policy = "RecruiterOrAdmin")]
    public async Task<IActionResult> Apply([FromBody] ApplyRequest request)
    {
        await _service.ApplyAsync(request.CandidateId, request.JobId);
        return Ok();
    }

    // UPDATE APPLICATION STATUS (CORE ATS FEATURE)
    [HttpPatch("{id:guid}/status")]
    [Authorize(Policy = "AssignedApplication")]
    public async Task<IActionResult> UpdateStatus(
        Guid id,
        [FromBody] UpdateStatusRequest request)
    {
        await _service.UpdateStatusAsync(id, request.Status, request.RecruiterId);
        return NoContent();
    }

    // GET APPLICATION TIMELINE
    [HttpGet("{applicationId:guid}/timeline")]
    [Authorize(Policy = "AssignedApplication")]
    public async Task<IActionResult> GetTimeline(Guid applicationId)
    {
        var timeline = await _service.GetTimelineAsync(applicationId);
        return Ok(timeline);
    }

    // GET APPLICATION AUDIT LOGS
    [HttpGet("{applicationId:guid}/audit")]
    [Authorize(Policy = "AssignedApplication")]
    public async Task<IActionResult> GetAuditLogs(Guid applicationId)
    {
        var auditLogs = await _service.GetAuditLogsAsync(applicationId);
        return Ok(auditLogs);
    }

    // GET APPLICATION EVENTS
    [HttpGet("{applicationId:guid}/events")]
    [Authorize(Policy = "AssignedApplication")]
    public async Task<IActionResult> GetEvents(Guid applicationId)
    {
        var events = await _service.GetEventsAsync(applicationId);
        return Ok(events);
    }

    // REVERT APPLICATION STATUS
    [HttpPatch("{id:guid}/revert")]
    [Authorize(Policy = "Admin")]
    public async Task<IActionResult> RevertStatus(
        Guid id,
        [FromBody] RevertStatusRequest request)
    {
        await _service.RevertStatusAsync(id, request.TargetStatus, request.Reason, request.RecruiterId);
        return NoContent();
    }

    // ASSIGN RECRUITER TO APPLICATION
    [HttpPost("{id:guid}/assign")]
    [Authorize(Policy = "Admin")]
    public async Task<IActionResult> AssignRecruiter(
        Guid id,
        [FromBody] AssignRecruiterRequest request)
    {
        await _service.AssignRecruiterAsync(id, request.RecruiterId, request.Actor);
        return NoContent();
    }

    // REASSIGN RECRUITER TO APPLICATION
    [HttpPost("{id:guid}/reassign")]
    [Authorize(Policy = "Admin")]
    public async Task<IActionResult> ReassignRecruiter(
        Guid id,
        [FromBody] ReassignRecruiterRequest request)
    {
        await _service.ReassignRecruiterAsync(id, request.NewRecruiterId, request.Actor);
        return NoContent();
    }

    // ANALYTICS ENDPOINTS
    [HttpGet("{applicationId:guid}/analytics/time-spent")]
    [Authorize(Policy = "AssignedApplication")]
    public async Task<IActionResult> GetTimeSpentPerStage(Guid applicationId)
    {
        var analytics = await _service.GetTimeSpentPerStageAsync(applicationId);
        return Ok(analytics);
    }

    [HttpGet("job/{jobId:guid}/analytics/drop-off")]
    [Authorize(Policy = "RecruiterOrAdmin")]
    public async Task<IActionResult> GetDropOffRates(Guid jobId)
    {
        var rates = await _service.GetDropOffRatesAsync(jobId);
        return Ok(rates);
    }

    [HttpGet("job/{jobId:guid}/analytics/funnel")]
    [Authorize(Policy = "RecruiterOrAdmin")]
    public async Task<IActionResult> GetConversionFunnel(Guid jobId)
    {
        var funnel = await _service.GetConversionFunnelAsync(jobId);
        return Ok(funnel);
    }
}

// Request DTOs
public record ApplyRequest(Guid CandidateId, Guid JobId);

public record UpdateStatusRequest(ApplicationStatus Status, string RecruiterId);

public record RevertStatusRequest(ApplicationStatus TargetStatus, string Reason, string RecruiterId);

public record AssignRecruiterRequest(Guid RecruiterId, string Actor);

public record ReassignRecruiterRequest(Guid NewRecruiterId, string Actor);
