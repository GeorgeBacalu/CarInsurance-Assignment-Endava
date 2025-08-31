using NCrontab;

namespace CarInsurance.Api.Workers;
public abstract class BaseBackgroundWorker : BackgroundService
{
    protected string ServiceName { get; }
    protected IServiceScopeFactory ScopeFactory { get; }

    private readonly CrontabSchedule _schedule;
    private DateTime _nextRun;

    protected BaseBackgroundWorker(IServiceScopeFactory scopeFactory, string serviceName, string cron)
    {
        (ScopeFactory, ServiceName) = (scopeFactory, serviceName);
        _schedule = CrontabSchedule.Parse(cron, new CrontabSchedule.ParseOptions { IncludingSeconds = true });
        _nextRun = _schedule.GetNextOccurrence(DateTime.UtcNow);
    }

    protected abstract Task RunIteration(CancellationToken token);
    protected virtual void WarmUp() { }
    protected virtual void HandleIterationError(Exception exception) { }

    protected override async Task ExecuteAsync(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            if (DateTime.UtcNow >= _nextRun)
            {
                WarmUp();
                try
                {
                    await RunIteration(token);
                }
                catch (Exception exception)
                {
                    HandleIterationError(exception);
                }
                _nextRun = _schedule.GetNextOccurrence(DateTime.UtcNow);
            }
            await Task.Delay(1000, token);
        }
    }
}
