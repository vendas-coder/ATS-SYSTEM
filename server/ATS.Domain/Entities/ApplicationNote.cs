using ATS.Domain.Enums;

namespace ATS.Domain.Entities;

public class ApplicationNote
{
    public Guid Id { get; set; }

    // SINGLE FK
    public Guid JobApplicationId { get; set; }

    // Status of application when note was added
    public ApplicationStatus Status { get; set; }

    public string Note { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    //  SINGLE navigation
    public JobApplication JobApplication { get; set; } = null!;
}
