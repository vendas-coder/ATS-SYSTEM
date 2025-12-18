using ATS.Domain.Entities;

namespace ATS.Application.Interfaces;

public interface IApplicationEventRepository
{
    Task<IEnumerable<ApplicationEvent>> GetByApplicationIdAsync(Guid applicationId);
    Task AddAsync(ApplicationEvent applicationEvent);
}