using ATS.Application.Interfaces;
using ATS.Domain.Entities;
using ATS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ATS.Infrastructure.Repositories.Ef;

public class EfApplicationRepository : IApplicationRepository
{
    private readonly ATSDbContext _context;

    public EfApplicationRepository(ATSDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<JobApplication>> GetByJobIdAsync(Guid jobId)
    {
        return await _context.Applications
            .Where(a => a.JobId == jobId)
            .ToListAsync();
    }

    public async Task<JobApplication?> GetByIdAsync(Guid id)
    {
        return await _context.Applications
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<JobApplication?> GetByIdWithNotesAsync(Guid id)
    {
        return await _context.Applications
            .Include(a => a.Notes)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<IEnumerable<JobApplication>> GetAllAsync()
    {
        return await _context.Applications.ToListAsync();
    }

    public IQueryable<JobApplication> Query()
    {
        return _context.Applications.AsQueryable();
    }

    public async Task AddAsync(JobApplication application)
    {
        _context.Applications.Add(application);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(JobApplication application)
    {
        _context.Applications.Update(application);
        await _context.SaveChangesAsync();
    }
}
