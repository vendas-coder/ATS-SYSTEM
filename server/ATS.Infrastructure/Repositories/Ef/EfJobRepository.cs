using ATS.Application.Interfaces;
using ATS.Domain.Entities;
using ATS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ATS.Infrastructure.Repositories.Ef;

public class EfJobRepository : IJobRepository
{
    private readonly ATSDbContext _context;

    public EfJobRepository(ATSDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Job>> GetAllAsync()
    {
        return await _context.Jobs.ToListAsync();
    }

    public async Task<Job?> GetByIdAsync(Guid id)
    {
        return await _context.Jobs.FirstOrDefaultAsync(j => j.Id == id);
    }

    public async Task AddAsync(Job job)
    {
        _context.Jobs.Add(job);
        await _context.SaveChangesAsync();
    }
}
