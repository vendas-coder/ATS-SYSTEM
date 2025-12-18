using ATS.Application.Interfaces;
using ATS.Domain.Entities;
using ATS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ATS.Infrastructure.Repositories.Ef;

public class EfApplicationNoteRepository : IApplicationNoteRepository
{
    private readonly ATSDbContext _context;

    public EfApplicationNoteRepository(ATSDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(ApplicationNote note)
    {
        _context.ApplicationNotes.Add(note);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<ApplicationNote>> GetByApplicationIdAsync(Guid applicationId)
    {
       return await _context.ApplicationNotes
    .Where(n => n.JobApplicationId == applicationId)
    .OrderByDescending(n => n.CreatedAt)
    .ToListAsync();
    }
}
