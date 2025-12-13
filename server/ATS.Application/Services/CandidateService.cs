using ATS.Application.Interfaces;
using ATS.Domain.Entities;

namespace ATS.Application.Services;

public class CandidateService
{
    private readonly ICandidateRepository _repository;

    public CandidateService(ICandidateRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<Candidate>> GetAllAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<Candidate?> GetByIdAsync(Guid id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task CreateAsync(Candidate candidate)
    {
        candidate.Id = Guid.NewGuid();
        await _repository.AddAsync(candidate);
    }
}
