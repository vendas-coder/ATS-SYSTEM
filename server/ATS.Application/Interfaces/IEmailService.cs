using ATS.Domain.Entities;
using ATS.Domain.Enums;

namespace ATS.Application.Interfaces;

public interface IEmailService
{
    Task SendStatusChangeEmailAsync(JobApplication application, ApplicationStatus newStatus, string? recipientEmail = null);
}