using ATS.Application.Services;
using Microsoft.AspNetCore.Mvc;

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
    public async Task<IActionResult> GetByJob(Guid jobId)
    {
        var applications = await _service.GetByJobIdAsync(jobId);
        return Ok(applications);
    }

    // Apply a candidate to a job
    [HttpPost]
    public async Task<IActionResult> Apply([FromBody] ApplyRequest request)
    {
        await _service.ApplyAsync(request.CandidateId, request.JobId);
        return Ok();
    }
}

// Simple request DTO (kept inside controller for now)
public record ApplyRequest(Guid CandidateId, Guid JobId);
