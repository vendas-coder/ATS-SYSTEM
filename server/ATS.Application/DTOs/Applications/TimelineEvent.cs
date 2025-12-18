namespace ATS.Application.DTOs.Applications;

public class TimelineEvent
{
    public string Type { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string Description { get; set; } = string.Empty;
}