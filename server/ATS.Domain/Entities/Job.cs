namespace ATS.Domain.Entities;

public class Job
{
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;

    public bool IsOpen { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
