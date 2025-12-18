using ATS.Domain.Enums;

namespace ATS.Application.DTOs.Applications;

public class RevertStatusRequest
{
    public ApplicationStatus TargetStatus { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string RecruiterId { get; set; } = string.Empty;
}