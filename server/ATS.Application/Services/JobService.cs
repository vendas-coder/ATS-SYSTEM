using ATS.Application.Interfaces;
using ATS.Domain.Entities;

namespace ATS.Application.Services;

public class JobService
{
    private readonly IJobRepository _repository;

    public JobService(IJobRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<Job>> GetAllAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<Job?> GetByIdAsync(Guid id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task CreateAsync(Job job)
    {
        job.Id = Guid.NewGuid();
        await _repository.AddAsync(job);
    }
}
