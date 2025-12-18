using ATS.Domain.Enums;

namespace ATS.Application.DTOs.Applications;

public class HiringFunnelStageDto
{
    public ApplicationStatus Status { get; set; }
    public int Count { get; set; }
}