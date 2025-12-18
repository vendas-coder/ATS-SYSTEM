using ATS.Domain.Enums;

namespace ATS.Application.DTOs.Applications;

public class ApplicationAuditLogResponse
{
    public Guid Id { get; set; }
    public string Action { get; set; } = string.Empty;
    public ApplicationStatus? OldStatus { get; set; }
    public ApplicationStatus? NewStatus { get; set; }
    public string RecruiterId { get; set; } = string.Empty;
    public string? Reason { get; set; }
    public DateTime CreatedAt { get; set; }
}