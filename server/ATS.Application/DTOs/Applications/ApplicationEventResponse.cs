using ATS.Domain.Enums;

namespace ATS.Application.DTOs.Applications;

public class ApplicationEventResponse
{
    public Guid Id { get; set; }
    public ApplicationStatus? OldStatus { get; set; }
    public ApplicationStatus? NewStatus { get; set; }
    public string ActionType { get; set; } = string.Empty;
    public string Actor { get; set; } = string.Empty;
    public string? Reason { get; set; }
    public DateTime CreatedAt { get; set; }
}