using ATS.Application.DTOs.Candidates;
using ATS.Application.Services;
using ATS.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ATS.API.Controllers;

[ApiController]
[Route("api/candidates")]
[Authorize(Policy = "RecruiterOrAdmin")]
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
public async Task<IActionResult> Create(CreateCandidateRequest request)
{
    var candidate = new Candidate
    {
        FirstName = request.FirstName,
        LastName = request.LastName,
        Email = request.Email,
        Phone = request.Phone
    };

    await _service.CreateAsync(candidate);
    return Ok(candidate);
}

}
