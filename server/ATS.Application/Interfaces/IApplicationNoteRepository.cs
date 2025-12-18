using ATS.Domain.Entities;

namespace ATS.Application.Interfaces;

public interface IApplicationNoteRepository
{
    Task AddAsync(ApplicationNote note);
    Task<IEnumerable<ApplicationNote>> GetByApplicationIdAsync(Guid applicationId);
}
