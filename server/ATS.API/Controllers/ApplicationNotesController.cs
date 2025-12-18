using ATS.Application.Services;
using ATS.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ATS.API.Controllers;

[ApiController]
[Route("api/applications/{applicationId:guid}/notes")]
public class ApplicationNotesController : ControllerBase
{
    private readonly ApplicationNoteService _service;

    public ApplicationNotesController(ApplicationNoteService service)
    {
        _service = service;
    }

    /// <summary>
    /// Add a recruiter note for a specific application stage
    /// </summary>
    [HttpPost]
    [Authorize(Policy = "AssignedApplication")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> AddNote(
        [FromRoute] Guid applicationId,
        [FromBody] AddNoteRequest request)
    {
        await _service.AddNoteAsync(
            applicationId,
            request.Status,
            request.Note,
            request.RecruiterId
        );

        return NoContent();
    }

    /// <summary>
    /// Get all recruiter notes for an application
    /// </summary>
    [HttpGet]
    [Authorize(Policy = "AssignedApplication")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetNotes(
        [FromRoute] Guid applicationId)
    {
        var notes = await _service.GetNotesAsync(applicationId);
        return Ok(notes);
    }
}

/// <summary>
/// Request DTO for adding application notes
/// </summary>
public record AddNoteRequest(
    ApplicationStatus Status,
    string Note,
    string RecruiterId
);
