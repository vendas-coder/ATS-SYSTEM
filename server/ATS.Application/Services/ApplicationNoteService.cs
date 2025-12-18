using ATS.Application.Interfaces;
using ATS.Domain.Entities;
using ATS.Domain.Enums;
using System.Text.Json;
using System.Threading.Tasks;

namespace ATS.Application.Services;

public class ApplicationNoteService
{
    private readonly IApplicationNoteRepository _repository;
    private readonly IApplicationAuditLogRepository _auditRepository;
    private readonly IApplicationEventRepository _eventRepository;

    public ApplicationNoteService(IApplicationNoteRepository repository, IApplicationAuditLogRepository auditRepository, IApplicationEventRepository eventRepository)
    {
        _repository = repository;
        _auditRepository = auditRepository;
        _eventRepository = eventRepository;
    }

    public async Task AddNoteAsync(
        Guid applicationId,
        ApplicationStatus status,
        string note,
        string recruiterId)
    {
        var applicationNote = new ApplicationNote
        {
            Id = Guid.NewGuid(),
            JobApplicationId = applicationId,
            Status = status,
            Note = note,
            CreatedAt = DateTime.UtcNow
        };

        await _repository.AddAsync(applicationNote);

        // Log audit
        var auditLog = new ApplicationAuditLog
        {
            Id = Guid.NewGuid(),
            ApplicationId = applicationId,
            Action = "Note Added",
            OldStatus = null,
            NewStatus = null,
            RecruiterId = recruiterId,
            CreatedAt = DateTime.UtcNow
        };

        await _auditRepository.AddAsync(auditLog);

        // Log event
        var applicationEvent = new ApplicationEvent
        {
            Id = Guid.NewGuid(),
            ApplicationId = applicationId,
            OldStatus = null,
            NewStatus = status,
            ActionType = "NoteAdded",
            Actor = recruiterId,
            Reason = note,
            CreatedAt = DateTime.UtcNow
        };

        await _eventRepository.AddAsync(applicationEvent);
    }

    public async Task<IEnumerable<ApplicationNote>> GetNotesAsync(Guid applicationId)
    {
        return await _repository.GetByApplicationIdAsync(applicationId);
    }
}
