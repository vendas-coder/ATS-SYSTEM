using ATS.Domain.Enums;

namespace ATS.Domain.Entities;

public class Application
{
    public Guid Id { get; set; }

    public Guid CandidateId { get; set; }
    public Guid JobId { get; set; }

    public ApplicationStatus Status { get; set; } = ApplicationStatus.Applied;

    public DateTime AppliedAt { get; set; } = DateTime.UtcNow;
}
