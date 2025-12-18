using ATS.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace ATS.API.Authorization;

public class AssignedApplicationHandler : AuthorizationHandler<AssignedApplicationRequirement>
{
    private readonly IServiceProvider _serviceProvider;

    public AssignedApplicationHandler(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, AssignedApplicationRequirement requirement)
    {
        var httpContext = context.Resource as HttpContext;
        if (httpContext == null)
        {
            context.Fail();
            return;
        }

        // Get application ID from route
        var routeData = httpContext.GetRouteData();
        if (!routeData.Values.TryGetValue("id", out var idValue) || !Guid.TryParse(idValue?.ToString(), out var applicationId))
        {
            context.Fail();
            return;
        }

        // Get user ID (assuming it's stored in claims)
        var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
        {
            context.Fail();
            return;
        }

        // Check if user is admin (admins have full access)
        if (context.User.IsInRole("Admin"))
        {
            context.Succeed(requirement);
            return;
        }

        // For recruiters, check if application is assigned to them
        if (context.User.IsInRole("Recruiter"))
        {
            using var scope = _serviceProvider.CreateScope();
            var assignmentRepo = scope.ServiceProvider.GetRequiredService<IJobApplicationAssignmentRepository>();

            var assignments = await assignmentRepo.GetByApplicationIdAsync(applicationId);
            var recruiterId = Guid.Parse(userIdClaim.Value); // Assuming user ID is recruiter ID

            if (assignments.Any(a => a.RecruiterId == recruiterId))
            {
                context.Succeed(requirement);
                return;
            }
        }

        context.Fail();
    }
}