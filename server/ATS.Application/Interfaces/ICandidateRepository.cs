using ATS.Domain.Entities;

namespace ATS.Application.Interfaces;

public interface ICandidateRepository
{
    Task<IEnumerable<Candidate>> GetAllAsync();
    Task<Candidate?> GetByIdAsync(Guid id);
    Task AddAsync(Candidate candidate);
}
