using CarInsurance.Api.Data;
using CarInsurance.Api.Dtos.Common;
using CarInsurance.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace CarInsurance.Api.Workers;
public class PolicyExpirationLoggingWorker : BaseBackgroundWorker
{
    private readonly ILogger<PolicyExpirationLoggingWorker> _logger;

    public PolicyExpirationLoggingWorker(IServiceScopeFactory scopeFactory, ILogger<PolicyExpirationLoggingWorker> logger)
        : base(scopeFactory, nameof(PolicyExpirationLoggingWorker), "0 0 * * * *") => _logger = logger;

    protected override async Task RunIteration(CancellationToken token)
    {
        var windowStartTime = DateTime.UtcNow.AddHours(-1);
        var windowEndTime = DateTime.UtcNow;
        var endDateToCheck = DateOnly.FromDateTime(windowEndTime.Date.AddDays(-1));

        using var scope = ScopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var upcomingPolicyExpiry = await db.Policies
            .Where(p => p.EndDate == endDateToCheck)
            .Select(p => new InsurancePolicyDto(p.Id, p.Provider, p.StartDate, p.EndDate))
            .ToListAsync(token);

        foreach (var policy in upcomingPolicyExpiry)
        {
            var expiryTime = policy.EndDate.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc).AddDays(1);
            if (expiryTime < windowStartTime || expiryTime > windowEndTime)
                continue;

            var wasPolicyExpiryTracked = await db.PolicyExpiryLogs.AnyAsync(x => x.PolicyId == policy.Id, token);
            if (wasPolicyExpiryTracked)
                continue;

            var policyExpiryLog = new PolicyExpiryLog
            {
                PolicyId = policy.Id,
                EndDate = policy.EndDate,
                LoggedAt = windowEndTime
            };
            await db.PolicyExpiryLogs.AddAsync(policyExpiryLog, token);
            await db.SaveChangesAsync(token);

            _logger.LogInformation("{Policy} - expired", policy);
        }
    }
}
