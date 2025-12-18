using ATS.Domain.Enums;

namespace ATS.Application.DTOs.Applications;

public class FunnelStage
{
    public string Stage { get; set; } = string.Empty;
    public int Count { get; set; }
    public double ConversionPercentage { get; set; }
}

public class TimeToHireStage
{
    public string Stage { get; set; } = string.Empty;
    public TimeSpan AverageTime { get; set; }
}

public class HiringFunnelAnalytics
{
    public IEnumerable<FunnelStage> Stages { get; set; } = new List<FunnelStage>();
    public TimeSpan AverageTimeToHire { get; set; }
}

public class TimeToHireAnalytics
{
    public IEnumerable<TimeToHireStage> Stages { get; set; } = new List<TimeToHireStage>();
    public TimeSpan OverallAverageTimeToHire { get; set; }
}