using ATS.Application.Services;
using ATS.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace ATS.API.Controllers;

[ApiController]
[Route("api/jobs")]
public class JobsController : ControllerBase
{
    private readonly JobService _service;

    public JobsController(JobService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var jobs = await _service.GetAllAsync();
        return Ok(jobs);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var job = await _service.GetByIdAsync(id);
        if (job == null)
            return NotFound();

        return Ok(job);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Job job)
    {
        await _service.CreateAsync(job);
        return CreatedAtAction(nameof(GetById), new { id = job.Id }, job);
    }
}
