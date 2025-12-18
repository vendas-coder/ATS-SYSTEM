using ATS.Domain.Enums;

namespace ATS.Application.DTOs.Applications;

public class StageTimeSpent
{
    public ApplicationStatus Stage { get; set; }
    public TimeSpan TimeSpent { get; set; }
}

public class ApplicationStageAnalytics
{
    public Guid ApplicationId { get; set; }
    public IEnumerable<StageTimeSpent> Stages { get; set; } = new List<StageTimeSpent>();
}

public class DropOffRate
{
    public ApplicationStatus Stage { get; set; }
    public double Rate { get; set; } // percentage
}

public class ConversionFunnel
{
    public ApplicationStatus Stage { get; set; }
    public int Count { get; set; }
}