using ATS.Application.Interfaces;
using ATS.Domain.Entities;
using ATS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ATS.Infrastructure.Repositories.Ef;

public class EfJobApplicationAssignmentRepository : IJobApplicationAssignmentRepository
{
    private readonly ATSDbContext _context;

    public EfJobApplicationAssignmentRepository(ATSDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<JobApplicationAssignment>> GetByApplicationIdAsync(Guid applicationId)
    {
        return await _context.JobApplicationAssignments
            .Where(a => a.JobApplicationId == applicationId)
            .Include(a => a.Recruiter)
            .ToListAsync();
    }

    public async Task AddAsync(JobApplicationAssignment assignment)
    {
        _context.JobApplicationAssignments.Add(assignment);
        await _context.SaveChangesAsync();
    }
}