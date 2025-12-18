using ATS.Domain.Enums;

namespace ATS.Domain.Entities;

public class ApplicationEvent
{
    public Guid Id { get; set; }
    public Guid ApplicationId { get; set; }
    public ApplicationStatus? OldStatus { get; set; }
    public ApplicationStatus? NewStatus { get; set; }
    public Guid? OldRecruiterId { get; set; }
    public Guid? NewRecruiterId { get; set; }
    public string ActionType { get; set; } = string.Empty;
    public string Actor { get; set; } = string.Empty;
    public string? Reason { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public JobApplication JobApplication { get; set; } = null!;
}