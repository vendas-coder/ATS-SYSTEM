using ATS.Domain.Entities;

namespace ATS.Application.Interfaces;

public interface IJobApplicationAssignmentRepository
{
    Task<IEnumerable<JobApplicationAssignment>> GetByApplicationIdAsync(Guid applicationId);
    Task AddAsync(JobApplicationAssignment assignment);
}