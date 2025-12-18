using ATS.Domain.Entities;

namespace ATS.Application.Interfaces;

public interface IRecruiterRepository
{
    Task<IEnumerable<Recruiter>> GetAllAsync();
    Task<Recruiter?> GetByIdAsync(Guid id);
    Task AddAsync(Recruiter recruiter);
    Task UpdateAsync(Recruiter recruiter);
}