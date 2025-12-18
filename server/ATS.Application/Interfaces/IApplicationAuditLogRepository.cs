using ATS.Domain.Entities;

namespace ATS.Application.Interfaces;

public interface IApplicationAuditLogRepository
{
    Task<IEnumerable<ApplicationAuditLog>> GetByApplicationIdAsync(Guid applicationId);
    Task AddAsync(ApplicationAuditLog auditLog);
}