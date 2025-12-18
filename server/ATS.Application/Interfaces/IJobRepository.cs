using ATS.Domain.Entities;

namespace ATS.Application.Interfaces;

public interface IJobRepository
{
    Task<IEnumerable<Job>> GetAllAsync();
    Task<Job?> GetByIdAsync(Guid id);
    Task AddAsync(Job job);
}
