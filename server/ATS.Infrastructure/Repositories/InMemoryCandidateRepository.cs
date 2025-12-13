using ATS.Application.Interfaces;
using ATS.Domain.Entities;

namespace ATS.Infrastructure.Repositories;

public class InMemoryCandidateRepository : ICandidateRepository
{
    private static readonly List<Candidate> _candidates = new();

    public Task<IEnumerable<Candidate>> GetAllAsync()
    {
        return Task.FromResult(_candidates.AsEnumerable());
    }

    public Task<Candidate?> GetByIdAsync(Guid id)
    {
        var candidate = _candidates.FirstOrDefault(c => c.Id == id);
        return Task.FromResult(candidate);
    }

    public Task AddAsync(Candidate candidate)
    {
        _candidates.Add(candidate);
        return Task.CompletedTask;
    }
}
