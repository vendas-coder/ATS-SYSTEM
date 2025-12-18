using ATS.Application.Interfaces;
using ATS.Domain.Entities;
using ATS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ATS.Infrastructure.Repositories.Ef;

public class EfApplicationEventRepository : IApplicationEventRepository
{
    private readonly ATSDbContext _context;

    public EfApplicationEventRepository(ATSDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ApplicationEvent>> GetByApplicationIdAsync(Guid applicationId)
    {
        return await _context.ApplicationEvents
            .Where(e => e.ApplicationId == applicationId)
            .OrderBy(e => e.CreatedAt)
            .ToListAsync();
    }

    public async Task AddAsync(ApplicationEvent applicationEvent)
    {
        _context.ApplicationEvents.Add(applicationEvent);
        await _context.SaveChangesAsync();
    }
}