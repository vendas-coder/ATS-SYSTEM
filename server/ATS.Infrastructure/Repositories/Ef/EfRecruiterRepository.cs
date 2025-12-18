using ATS.Application.Interfaces;
using ATS.Domain.Entities;
using ATS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ATS.Infrastructure.Repositories.Ef;

public class EfRecruiterRepository : IRecruiterRepository
{
    private readonly ATSDbContext _context;

    public EfRecruiterRepository(ATSDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Recruiter>> GetAllAsync()
    {
        return await _context.Recruiters.ToListAsync();
    }

    public async Task<Recruiter?> GetByIdAsync(Guid id)
    {
        return await _context.Recruiters.FindAsync(id);
    }

    public async Task AddAsync(Recruiter recruiter)
    {
        _context.Recruiters.Add(recruiter);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Recruiter recruiter)
    {
        _context.Recruiters.Update(recruiter);
        await _context.SaveChangesAsync();
    }
}