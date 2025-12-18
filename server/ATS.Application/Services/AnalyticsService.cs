using ATS.Application.DTOs.Applications;
using ATS.Application.Interfaces;
using ATS.Domain.Entities;
using ATS.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace ATS.Application.Services;

public class AnalyticsService
{
    private readonly IApplicationRepository _applicationRepository;
    private readonly IApplicationEventRepository _eventRepository;

    public AnalyticsService(IApplicationRepository applicationRepository, IApplicationEventRepository eventRepository)
    {
        _applicationRepository = applicationRepository;
        _eventRepository = eventRepository;
    }

    public async Task<IEnumerable<HiringFunnelStageDto>> GetHiringFunnelAsync()
    {
        var grouped = await _applicationRepository.Query()
            .GroupBy(a => a.Status)
            .Select(g => new { Status = g.Key, Count = g.Count() })
            .ToListAsync();
        return grouped.OrderBy(g => g.Status).Select(g => new HiringFunnelStageDto { Status = g.Status, Count = g.Count });
    }

    public async Task<TimeToHireAnalytics> GetTimeToHireAnalyticsAsync()
    {
        var allApplications = await _applicationRepository.GetAllAsync();
        var allEvents = new List<ApplicationEvent>();
        foreach (var app in allApplications)
        {
            var events = await _eventRepository.GetByApplicationIdAsync(app.Id);
            allEvents.AddRange(events);
        }

        var stages = new List<ApplicationStatus> { ApplicationStatus.Applied, ApplicationStatus.Screening, ApplicationStatus.Interview, ApplicationStatus.Offer };
        var timeStages = new List<TimeToHireStage>();

        foreach (var stage in stages)
        {
            var stageTimes = new List<TimeSpan>();
            foreach (var app in allApplications)
            {
                var appEvents = new List<ApplicationEvent>();
                foreach (var e in allEvents)
                {
                    if (e.ApplicationId == app.Id && (e.ActionType == "StatusChanged" || e.ActionType == "StatusReverted"))
                    {
                        appEvents.Add(e);
                    }
                }
                appEvents.Sort((a, b) => a.CreatedAt.CompareTo(b.CreatedAt));
                ApplicationEvent? entry = null;
                ApplicationEvent? exit = null;
                foreach (var e in appEvents)
                {
                    if (e.NewStatus == stage) entry = e;
                    if (e.OldStatus == stage) exit = e;
                }
                if (entry != null && exit != null)
                {
                    stageTimes.Add(exit.CreatedAt - entry.CreatedAt);
                }
                else if (entry != null && appEvents.Count > 0 && appEvents[appEvents.Count - 1].NewStatus == stage)
                {
                    stageTimes.Add(DateTime.UtcNow - entry.CreatedAt);
                }
            }
            long sumTicks = 0;
            foreach (var t in stageTimes) sumTicks += t.Ticks;
            var avg = stageTimes.Count > 0 ? TimeSpan.FromTicks(sumTicks / stageTimes.Count) : TimeSpan.Zero;
            timeStages.Add(new TimeToHireStage { Stage = stage.ToString(), AverageTime = avg });
        }

        var hiredApps = new List<Guid>();
        foreach (var e in allEvents)
        {
            if (e.NewStatus == ApplicationStatus.Hired && !hiredApps.Contains(e.ApplicationId))
            {
                hiredApps.Add(e.ApplicationId);
            }
        }
        var overallTimes = new List<TimeSpan>();
        foreach (var appId in hiredApps)
        {
            var appEvents = new List<ApplicationEvent>();
            foreach (var e in allEvents)
            {
                if (e.ApplicationId == appId) appEvents.Add(e);
            }
            appEvents.Sort((a, b) => a.CreatedAt.CompareTo(b.CreatedAt));
            ApplicationEvent? applied = null;
            ApplicationEvent? hired = null;
            foreach (var e in appEvents)
            {
                if (e.ActionType == "ApplicationCreated") applied = e;
                if (e.NewStatus == ApplicationStatus.Hired) hired = e;
            }
            if (applied != null && hired != null)
            {
                overallTimes.Add(hired.CreatedAt - applied.CreatedAt);
            }
        }
        long totalTicks = 0;
        foreach (var t in overallTimes) totalTicks += t.Ticks;
        var overallAvg = overallTimes.Count > 0 ? TimeSpan.FromTicks(totalTicks / overallTimes.Count) : TimeSpan.Zero;

        return new TimeToHireAnalytics { Stages = timeStages, OverallAverageTimeToHire = overallAvg };
    }
}