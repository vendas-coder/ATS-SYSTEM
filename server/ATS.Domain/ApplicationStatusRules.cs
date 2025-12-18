using ATS.Domain.Enums;

namespace ATS.Domain;

public static class ApplicationStatusRules
{
    private static readonly Dictionary<ApplicationStatus, HashSet<ApplicationStatus>> AllowedTransitions = new()
    {
        [ApplicationStatus.Applied] = new() { ApplicationStatus.Screening, ApplicationStatus.Rejected },
        [ApplicationStatus.Screening] = new() { ApplicationStatus.Interview, ApplicationStatus.Rejected },
        [ApplicationStatus.Interview] = new() { ApplicationStatus.Offer, ApplicationStatus.Rejected },
        [ApplicationStatus.Offer] = new() { ApplicationStatus.Hired, ApplicationStatus.Rejected },
        [ApplicationStatus.Hired] = new(),
        [ApplicationStatus.Rejected] = new()
    };

    public static void ValidateTransition(ApplicationStatus current, ApplicationStatus next)
    {
        if (current == next)
        {
            throw new InvalidOperationException($"Cannot set the same status: {current}");
        }

        if (!AllowedTransitions[current].Contains(next))
        {
            throw new InvalidOperationException($"Invalid status transition: {current} → {next}. Allowed transitions from {current}: {string.Join(", ", AllowedTransitions[current])}");
        }
    }

    public static bool CanRevertFrom(ApplicationStatus status)
    {
        return status != ApplicationStatus.Hired && status != ApplicationStatus.Rejected;
    }

    public static void ValidateRevert(ApplicationStatus current, ApplicationStatus target, string reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
        {
            throw new InvalidOperationException("Revert reason is required");
        }

        if (!CanRevertFrom(current))
        {
            throw new InvalidOperationException("Cannot revert from final status (Hired or Rejected)");
        }

        if (current == target)
        {
            throw new InvalidOperationException($"Status is already {target}");
        }

        // For revert, we allow going back to any previous status, but perhaps restrict to valid workflow.
        // Since the requirements don't specify, and to prevent invalid states, maybe allow revert to any except forward or something.
        // But to keep simple, allow revert to any status except the same and perhaps not forward.
        // Actually, since it's revert, probably allow to any previous in the chain.
        // But for simplicity, since the code allows any target, I'll keep it, but add check that target is not forward from current.

        // To prevent skipping or invalid, perhaps only allow revert to immediate previous or something.
        // But the existing code allows any target, so perhaps keep it open, but ensure not setting to Hired or Rejected if not allowed.

        // Wait, perhaps for revert, allow setting to any status except Hired and Rejected, since those are final.

        if (target == ApplicationStatus.Hired || target == ApplicationStatus.Rejected)
        {
            throw new InvalidOperationException("Cannot revert to final status (Hired or Rejected)");
        }

        // Also, prevent forward transitions in revert? But since it's revert, probably allow any back.

        // To make it strict, perhaps define allowed revert targets.

        // For now, I'll assume any target except same, final, and perhaps not forward.

        // Check if target is forward from current.

        if (IsForward(current, target))
        {
            throw new InvalidOperationException($"Cannot revert forward: {current} to {target}");
        }
    }

    private static bool IsForward(ApplicationStatus from, ApplicationStatus to)
    {
        // Simple check: if to has higher value, but Rejected has 5, Hired 4.

        // Better to check if to is in allowed from from.

        return AllowedTransitions[from].Contains(to);
    }
}