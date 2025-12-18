using ATS.Application.Interfaces;
using ATS.Domain.Entities;
using ATS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ATS.Infrastructure.Repositories.Ef;

public class EfApplicationAuditLogRepository : IApplicationAuditLogRepository
{
    private readonly ATSDbContext _context;

    public EfApplicationAuditLogRepository(ATSDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ApplicationAuditLog>> GetByApplicationIdAsync(Guid applicationId)
    {
        return await _context.ApplicationAuditLogs
            .Where(a => a.ApplicationId == applicationId)
            .OrderBy(a => a.CreatedAt)
            .ToListAsync();
    }

    public async Task AddAsync(ApplicationAuditLog auditLog)
    {
        _context.ApplicationAuditLogs.Add(auditLog);
        await _context.SaveChangesAsync();
    }
}