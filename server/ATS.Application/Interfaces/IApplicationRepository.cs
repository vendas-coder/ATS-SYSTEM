using ATS.Domain.Entities;

namespace ATS.Application.Interfaces;

public interface IApplicationRepository
{
    Task<IEnumerable<JobApplication>> GetByJobIdAsync(Guid jobId);
    Task<JobApplication?> GetByIdAsync(Guid id);
    Task<JobApplication?> GetByIdWithNotesAsync(Guid id);
    Task<IEnumerable<JobApplication>> GetAllAsync();
    IQueryable<JobApplication> Query();
    Task AddAsync(JobApplication application);
    Task UpdateAsync(JobApplication application);
}
