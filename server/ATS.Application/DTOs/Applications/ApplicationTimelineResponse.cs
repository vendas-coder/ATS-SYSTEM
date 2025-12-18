namespace ATS.Application.DTOs.Applications;

public class ApplicationTimelineResponse
{
    public Guid ApplicationId { get; set; }
    public string CurrentStatus { get; set; } = string.Empty;
    public List<TimelineEvent> Events { get; set; } = new();
}