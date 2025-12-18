using ATS.Domain.Enums;

namespace ATS.Domain.Entities;

public class JobApplication
{
    public Guid Id { get; set; }

    public Guid CandidateId { get; set; }
    public Guid JobId { get; set; }

    public ApplicationStatus Status { get; set; } = ApplicationStatus.Applied;
public ICollection<ApplicationNote> Notes { get; set; } = new List<ApplicationNote>();
    public ICollection<ApplicationAuditLog> AuditLogs { get; set; } = new List<ApplicationAuditLog>();
    public ICollection<ApplicationEvent> Events { get; set; } = new List<ApplicationEvent>();
    public ICollection<JobApplicationAssignment> Assignments { get; set; } = new List<JobApplicationAssignment>();

    public DateTime AppliedAt { get; set; } = DateTime.UtcNow;
}
