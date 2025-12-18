using ATS.Domain.Enums;

namespace ATS.Application.DTOs.Applications;

public class ApplicationResponse
{
    public Guid Id { get; set; }
    public Guid CandidateId { get; set; }
    public Guid JobId { get; set; }
    public ApplicationStatus Status { get; set; }
    public DateTime AppliedAt { get; set; }
}
