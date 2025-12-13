using ATS.Application.Services;
using ATS.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace ATS.API.Controllers;

[ApiController]
[Route("api/candidates")]
public class CandidatesController : ControllerBase
{
    private readonly CandidateService _service;

    public CandidatesController(CandidateService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var candidates = await _service.GetAllAsync();
        return Ok(candidates);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Candidate candidate)
    {
        await _service.CreateAsync(candidate);
        return CreatedAtAction(nameof(GetAll), candidate);
    }
}
