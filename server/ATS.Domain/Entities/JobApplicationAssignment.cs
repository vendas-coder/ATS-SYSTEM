namespace ATS.Domain.Entities;

public class JobApplicationAssignment
{
    public Guid Id { get; set; }
    public Guid JobApplicationId { get; set; }
    public Guid RecruiterId { get; set; }
    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public JobApplication JobApplication { get; set; } = null!;
    public Recruiter Recruiter { get; set; } = null!;
}