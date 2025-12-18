using ATS.Application.Interfaces;
using ATS.Domain.Entities;
using ATS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ATS.Infrastructure.Repositories.Ef;

public class EfCandidateRepository : ICandidateRepository
{
    private readonly ATSDbContext _context;

    public EfCandidateRepository(ATSDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Candidate>> GetAllAsync()
    {
        return await _context.Candidates.ToListAsync();
    }

    public async Task<Candidate?> GetByIdAsync(Guid id)
    {
        return await _context.Candidates.FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task AddAsync(Candidate candidate)
    {
        _context.Candidates.Add(candidate);
        await _context.SaveChangesAsync();
    }
}
