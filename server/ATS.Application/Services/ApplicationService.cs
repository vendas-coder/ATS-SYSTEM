using Microsoft.Extensions.Logging;
using ATS.Application.DTOs.Applications;
using ATS.Application.Interfaces;
using ATS.Domain;
using ATS.Domain.Entities;
using ATS.Domain.Enums;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace ATS.Application.Services;

public class ApplicationService
{
    private readonly IApplicationRepository _repository;
    private readonly IApplicationAuditLogRepository _auditRepository;
    private readonly IApplicationEventRepository _eventRepository;
    private readonly IRecruiterRepository _recruiterRepository;
    private readonly IJobApplicationAssignmentRepository _assignmentRepository;
    private readonly IEmailService _emailService;
    private readonly EmailOptions _emailOptions;
    private readonly ILogger<ApplicationService> _logger;

    public ApplicationService(
        IApplicationRepository repository,
        IApplicationAuditLogRepository auditRepository,
        IApplicationEventRepository eventRepository,
        IRecruiterRepository recruiterRepository,
        IJobApplicationAssignmentRepository assignmentRepository,
        IEmailService emailService,
        IOptions<EmailOptions> emailOptions,
        ILogger<ApplicationService> logger)
    {
        _repository = repository;
        _auditRepository = auditRepository;
        _eventRepository = eventRepository;
        _recruiterRepository = recruiterRepository;
        _assignmentRepository = assignmentRepository;
        _emailService = emailService;
        _emailOptions = emailOptions.Value;
        _logger = logger;
    }

    // =======================
    // READ
    // =======================
    public async Task<IEnumerable<ApplicationResponse>> GetByJobIdAsync(Guid jobId)
    {
        var applications = await _repository.GetByJobIdAsync(jobId);

        return applications.Select(a => new ApplicationResponse
        {
            Id = a.Id,
            CandidateId = a.CandidateId,
            JobId = a.JobId,
            Status = a.Status,
            AppliedAt = a.AppliedAt
        });
    }

    // =======================
    // CREATE
    // =======================
    public async Task ApplyAsync(Guid candidateId, Guid jobId)
    {
        var application = new JobApplication
        {
            Id = Guid.NewGuid(),
            CandidateId = candidateId,
            JobId = jobId,
            Status = ApplicationStatus.Applied,
            AppliedAt = DateTime.UtcNow
        };

        try
        {
            await _repository.AddAsync(application);
            _logger.LogInformation("Application created: {ApplicationId} for Candidate {CandidateId} and Job {JobId}", application.Id, candidateId, jobId);

            // Log event
            await LogApplicationEventAsync(application.Id, null, null, null, null, "ApplicationCreated", "System");

            // Send email notification (fire-and-forget)
            if (_emailOptions.Enabled && ShouldSendEmailForStatus(ApplicationStatus.Applied))
            {
                _ = Task.Run(() => _emailService.SendStatusChangeEmailAsync(application, ApplicationStatus.Applied));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating application for Candidate {CandidateId} and Job {JobId}", candidateId, jobId);
            throw;
        }
    }

    // =======================
    // UPDATE STATUS (PIPELINE RULES)
    // =======================
    public async Task UpdateStatusAsync(Guid applicationId, ApplicationStatus nextStatus, string recruiterId)
    {
        var application = await _repository.GetByIdAsync(applicationId);

        if (application is null)
        {
            _logger.LogWarning("Attempted to update status for non-existent application {ApplicationId}", applicationId);
            throw new InvalidOperationException("Application not found");
        }

        ApplicationStatusRules.ValidateTransition(application.Status, nextStatus);

        var oldStatus = application.Status;
        application.Status = nextStatus;

        try
        {
            await _repository.UpdateAsync(application);

            // Log audit
            var auditLog = new ApplicationAuditLog
            {
                Id = Guid.NewGuid(),
                ApplicationId = applicationId,
                Action = "Status Changed",
                OldStatus = oldStatus,
                NewStatus = nextStatus,
                RecruiterId = recruiterId,
                CreatedAt = DateTime.UtcNow
            };

            await _auditRepository.AddAsync(auditLog);

            // Log event
            await LogApplicationEventAsync(applicationId, oldStatus, nextStatus, null, null, "StatusChanged", recruiterId);

            // Send email notification (fire-and-forget)
            if (_emailOptions.Enabled && ShouldSendEmailForStatus(nextStatus))
            {
                _ = Task.Run(() => _emailService.SendStatusChangeEmailAsync(application, nextStatus));
            }

            _logger.LogInformation("Application {ApplicationId} status updated from {OldStatus} to {NewStatus} by recruiter {RecruiterId}", applicationId, oldStatus, nextStatus, recruiterId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating status for application {ApplicationId}", applicationId);
            throw;
        }
    }

    private bool ShouldSendEmailForStatus(ApplicationStatus status)
    {
        return status switch
        {
            ApplicationStatus.Applied => true,
            ApplicationStatus.Interview => true,
            ApplicationStatus.Offer => true,
            ApplicationStatus.Rejected => true,
            _ => false
        };
    }

    // =======================
    // AUDIT LOGS
    // =======================
    public async Task<IEnumerable<ApplicationAuditLogResponse>> GetAuditLogsAsync(Guid applicationId)
    {
        var auditLogs = await _auditRepository.GetByApplicationIdAsync(applicationId);

        return auditLogs.Select(a => new ApplicationAuditLogResponse
        {
            Id = a.Id,
            Action = a.Action,
            OldStatus = a.OldStatus,
            NewStatus = a.NewStatus,
            RecruiterId = a.RecruiterId,
            Reason = a.Reason,
            CreatedAt = a.CreatedAt
        });
    }

    // =======================
    // REVERT STATUS
    // =======================
    public async Task RevertStatusAsync(Guid applicationId, ApplicationStatus targetStatus, string reason, string recruiterId)
    {
        var application = await _repository.GetByIdAsync(applicationId);

        if (application is null)
            throw new InvalidOperationException("Application not found");

        ApplicationStatusRules.ValidateRevert(application.Status, targetStatus, reason);

        var oldStatus = application.Status;
        application.Status = targetStatus;

        await _repository.UpdateAsync(application);

        // Log audit
        var auditLog = new ApplicationAuditLog
        {
            Id = Guid.NewGuid(),
            ApplicationId = applicationId,
            Action = "Status Reverted",
            OldStatus = oldStatus,
            NewStatus = targetStatus,
            RecruiterId = recruiterId,
            Reason = reason,
            CreatedAt = DateTime.UtcNow
        };

        await _auditRepository.AddAsync(auditLog);

        // Log event
        await LogApplicationEventAsync(applicationId, oldStatus, targetStatus, null, null, "StatusReverted", recruiterId, reason);
    }

    // =======================
    // TIMELINE
    // =======================
    public async Task<ApplicationTimelineResponse> GetTimelineAsync(Guid applicationId)
    {
        var application = await _repository.GetByIdWithNotesAsync(applicationId);

        if (application is null)
            throw new InvalidOperationException("Application not found");

        var events = new List<TimelineEvent>
        {
            new TimelineEvent
            {
                Type = "Applied",
                Status = ApplicationStatus.Applied.ToString(),
                Date = application.AppliedAt,
                Description = "Application submitted"
            }
        };

        foreach (var note in application.Notes.OrderBy(n => n.CreatedAt))
        {
            events.Add(new TimelineEvent
            {
                Type = "Note",
                Status = note.Status.ToString(),
                Date = note.CreatedAt,
                Description = note.Note
            });
        }

        return new ApplicationTimelineResponse
        {
            ApplicationId = application.Id,
            CurrentStatus = application.Status.ToString(),
            Events = events.OrderBy(e => e.Date).ToList()
        };
    }

    // =======================
    // EVENTS
    // =======================
    public async Task<IEnumerable<ApplicationEventResponse>> GetEventsAsync(Guid applicationId)
    {
        var events = await _eventRepository.GetByApplicationIdAsync(applicationId);

        return events.Select(e => new ApplicationEventResponse
        {
            Id = e.Id,
            OldStatus = e.OldStatus,
            NewStatus = e.NewStatus,
            ActionType = e.ActionType,
            Actor = e.Actor,
            Reason = e.Reason,
            CreatedAt = e.CreatedAt
        });
    }

    // =======================
    // ANALYTICS PROJECTIONS
    // =======================
    public async Task<ApplicationStageAnalytics> GetTimeSpentPerStageAsync(Guid applicationId)
    {
        var events = await _eventRepository.GetByApplicationIdAsync(applicationId);

        // Filter status change events
        var statusChanges = events
            .Where(e => e.ActionType == "StatusChanged" || e.ActionType == "StatusReverted")
            .OrderBy(e => e.CreatedAt)
            .ToList();

        var stages = new List<StageTimeSpent>();
        DateTime? previousTime = null;
        ApplicationStatus? previousStatus = null;

        foreach (var change in statusChanges)
        {
            if (previousTime.HasValue && previousStatus.HasValue)
            {
                var timeSpent = change.CreatedAt - previousTime.Value;
                stages.Add(new StageTimeSpent
                {
                    Stage = previousStatus.Value,
                    TimeSpent = timeSpent
                });
            }
            previousTime = change.CreatedAt;
            previousStatus = change.NewStatus;
        }

        // Add time for current stage if still in progress
        if (previousTime.HasValue && previousStatus.HasValue)
        {
            var currentTime = DateTime.UtcNow;
            var timeSpent = currentTime - previousTime.Value;
            stages.Add(new StageTimeSpent
            {
                Stage = previousStatus.Value,
                TimeSpent = timeSpent
            });
        }

        return new ApplicationStageAnalytics
        {
            ApplicationId = applicationId,
            Stages = stages
        };
    }

    public async Task<IEnumerable<DropOffRate>> GetDropOffRatesAsync(Guid jobId)
    {
        var applications = await _repository.GetByJobIdAsync(jobId);
        var appIds = applications.Select(a => a.Id).ToList();

        var allEvents = new List<ApplicationEvent>();
        foreach (var appId in appIds)
        {
            var events = await _eventRepository.GetByApplicationIdAsync(appId);
            allEvents.AddRange(events);
        }

        // Group by application and get final status
        var finalStatuses = allEvents
            .Where(e => e.ActionType == "StatusChanged" || e.ActionType == "StatusReverted")
            .GroupBy(e => e.ApplicationId)
            .Select(g => new
            {
                ApplicationId = g.Key,
                FinalStatus = g.OrderByDescending(e => e.CreatedAt).First().NewStatus
            })
            .ToDictionary(x => x.ApplicationId, x => x.FinalStatus);

        var totalApplications = appIds.Count;
        var dropOffRates = new List<DropOffRate>();

        // Calculate drop-off for each stage
        var stages = Enum.GetValues<ApplicationStatus>().Where(s => s != ApplicationStatus.Hired && s != ApplicationStatus.Rejected).ToList();

        foreach (var stage in stages)
        {
            var reachedStage = finalStatuses.Count(fs => fs.Value >= stage);
            var proceededToNext = finalStatuses.Count(fs => fs.Value > stage);

            if (reachedStage > 0)
            {
                var rate = (double)(reachedStage - proceededToNext) / reachedStage * 100;
                dropOffRates.Add(new DropOffRate
                {
                    Stage = stage,
                    Rate = Math.Round(rate, 2)
                });
            }
        }

        return dropOffRates;
    }

    public async Task<IEnumerable<ConversionFunnel>> GetConversionFunnelAsync(Guid jobId)
    {
        var applications = await _repository.GetByJobIdAsync(jobId);
        var appIds = applications.Select(a => a.Id).ToList();

        var allEvents = new List<ApplicationEvent>();
        foreach (var appId in appIds)
        {
            var events = await _eventRepository.GetByApplicationIdAsync(appId);
            allEvents.AddRange(events);
        }

        // Get the highest status reached for each application
        var maxStatuses = allEvents
            .Where(e => e.ActionType == "StatusChanged" || e.ActionType == "StatusReverted")
            .GroupBy(e => e.ApplicationId)
            .Select(g => g.OrderByDescending(e => e.CreatedAt).First().NewStatus ?? ApplicationStatus.Applied)
            .ToList();

        var funnel = new List<ConversionFunnel>();
        var stages = Enum.GetValues<ApplicationStatus>().OrderBy(s => (int)s).ToList();

        foreach (var stage in stages)
        {
            var count = maxStatuses.Count(s => (int)s >= (int)stage);
            funnel.Add(new ConversionFunnel
            {
                Stage = stage,
                Count = count
            });
        }

        return funnel;
    }

    private async Task LogApplicationEventAsync(Guid applicationId, ApplicationStatus? oldStatus, ApplicationStatus? newStatus, Guid? oldRecruiterId, Guid? newRecruiterId, string actionType, string actor, string? reason = null)
    {
        var applicationEvent = new ApplicationEvent
        {
            Id = Guid.NewGuid(),
            ApplicationId = applicationId,
            OldStatus = oldStatus,
            NewStatus = newStatus,
            OldRecruiterId = oldRecruiterId,
            NewRecruiterId = newRecruiterId,
            ActionType = actionType,
            Actor = actor,
            Reason = reason,
            CreatedAt = DateTime.UtcNow
        };

        await _eventRepository.AddAsync(applicationEvent);
    }

    public async Task AssignRecruiterAsync(Guid applicationId, Guid recruiterId, string actor)
    {
        var application = await _repository.GetByIdAsync(applicationId);
        if (application == null)
            throw new ArgumentException("Application not found");

        var recruiter = await _recruiterRepository.GetByIdAsync(recruiterId);
        if (recruiter == null)
            throw new ArgumentException("Recruiter not found");

        // Check if already assigned to this recruiter
        var existingAssignments = await _assignmentRepository.GetByApplicationIdAsync(applicationId);
        if (existingAssignments.Any(a => a.RecruiterId == recruiterId))
            throw new InvalidOperationException("Application is already assigned to this recruiter");

        var assignment = new JobApplicationAssignment
        {
            Id = Guid.NewGuid(),
            JobApplicationId = applicationId,
            RecruiterId = recruiterId,
            AssignedAt = DateTime.UtcNow
        };

        await _assignmentRepository.AddAsync(assignment);

        // Log event
        await LogApplicationEventAsync(applicationId, null, null, null, recruiterId, "RecruiterAssigned", actor, $"Assigned to recruiter {recruiter.Name}");
    }

    public async Task ReassignRecruiterAsync(Guid applicationId, Guid newRecruiterId, string actor)
    {
        var application = await _repository.GetByIdAsync(applicationId);
        if (application == null)
            throw new ArgumentException("Application not found");

        var newRecruiter = await _recruiterRepository.GetByIdAsync(newRecruiterId);
        if (newRecruiter == null)
            throw new ArgumentException("New recruiter not found");

        // Check if already assigned to this recruiter
        var existingAssignments = await _assignmentRepository.GetByApplicationIdAsync(applicationId);
        if (existingAssignments.Any(a => a.RecruiterId == newRecruiterId))
            throw new InvalidOperationException("Application is already assigned to this recruiter");

        var assignment = new JobApplicationAssignment
        {
            Id = Guid.NewGuid(),
            JobApplicationId = applicationId,
            RecruiterId = newRecruiterId,
            AssignedAt = DateTime.UtcNow
        };

        await _assignmentRepository.AddAsync(assignment);

        // Log event
        await LogApplicationEventAsync(applicationId, null, null, null, newRecruiterId, "RecruiterReassigned", actor, $"Reassigned to recruiter {newRecruiter.Name}");
    }
}
