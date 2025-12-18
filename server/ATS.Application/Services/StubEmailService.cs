using ATS.Application.Interfaces;
using ATS.Domain.Entities;
using ATS.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace ATS.Application.Services;

public class StubEmailService : IEmailService
{
    private readonly ILogger<StubEmailService> _logger;

    public StubEmailService(ILogger<StubEmailService> logger)
    {
        _logger = logger;
    }

    public async Task SendStatusChangeEmailAsync(JobApplication application, ApplicationStatus newStatus, string? recipientEmail = null)
    {
        // Simulate async email sending
        await Task.Delay(100);

        var subject = GetEmailSubject(newStatus);
        var body = GetEmailBody(application, newStatus);

        _logger.LogInformation("STUB EMAIL: To: {Email}, Subject: {Subject}, Body: {Body}",
            recipientEmail ?? "candidate@example.com", subject, body);
    }

    private string GetEmailSubject(ApplicationStatus status)
    {
        return status switch
        {
            ApplicationStatus.Applied => "Application Received",
            ApplicationStatus.Interview => "Interview Scheduled",
            ApplicationStatus.Offer => "Job Offer",
            ApplicationStatus.Rejected => "Application Update",
            _ => "Application Status Update"
        };
    }

    private string GetEmailBody(JobApplication application, ApplicationStatus status)
    {
        return status switch
        {
            ApplicationStatus.Applied => $"Dear Candidate,\n\nThank you for applying to position {application.JobId}. Your application has been received and is under review.\n\nBest regards,\nRecruitment Team",
            ApplicationStatus.Interview => $"Dear Candidate,\n\nCongratulations! Your application for position {application.JobId} has progressed to the interview stage. We will contact you soon with scheduling details.\n\nBest regards,\nRecruitment Team",
            ApplicationStatus.Offer => $"Dear Candidate,\n\nWe are pleased to offer you the position {application.JobId}. Please review the offer details and let us know your decision.\n\nBest regards,\nRecruitment Team",
            ApplicationStatus.Rejected => $"Dear Candidate,\n\nThank you for your interest in position {application.JobId}. After careful consideration, we have decided to move forward with other candidates at this time.\n\nBest regards,\nRecruitment Team",
            _ => $"Dear Candidate,\n\nYour application status for position {application.JobId} has been updated to {status}.\n\nBest regards,\nRecruitment Team"
        };
    }
}