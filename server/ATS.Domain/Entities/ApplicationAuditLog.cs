using ATS.Domain.Enums;

namespace ATS.Domain.Entities;

public class ApplicationAuditLog
{
    public Guid Id { get; set; }
    public Guid ApplicationId { get; set; }
    public string Action { get; set; } = string.Empty;
    public ApplicationStatus? OldStatus { get; set; }
    public ApplicationStatus? NewStatus { get; set; }
    public string RecruiterId { get; set; } = string.Empty;
    public string? Reason { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public JobApplication JobApplication { get; set; } = null!;
}