using Kip.Report.Domain.Models;
using Kip.Report.Domain.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Kip.Report.Application;

public class ReportBackgroundWorker : BackgroundService
{
    private readonly BackgroundReportQueue _queue;
    private readonly IServiceProvider _serviceProvider;
    private readonly TimeProvider _timeProvider;
    private readonly QueryOptions _options;

    public ReportBackgroundWorker(
        BackgroundReportQueue queue,
        IServiceProvider serviceProvider,
        TimeProvider timeProvider,
        IOptions<QueryOptions> options)
    {
        _queue = queue;
        _serviceProvider = serviceProvider;
        _timeProvider = timeProvider;
        _options = options.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var repo = scope.ServiceProvider.GetRequiredService<IQueryRepository>();
            var uncompletedIds = await repo.GetUncompletedAsync(cancellationToken);

            foreach (var id in uncompletedIds)
            {
                await repo.ResetStartAsync(id, cancellationToken);
                await _queue.QueueReportAsync(id, cancellationToken);
            }
        }

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                Guid queryId = await _queue.DequeueAsync(cancellationToken);
                _ = ProcessReportAsync(queryId, cancellationToken);
            }
            catch (OperationCanceledException) { break; }
        }
    }

    private async Task ProcessReportAsync(Guid queryId, CancellationToken stoppingToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<IQueryRepository>();

        await repo.StartAsync(queryId, _timeProvider.GetUtcNow(), stoppingToken);

        await Task.Delay(_options.Timeout, stoppingToken);

        var reportData = new ReportData { CountSignIn = 12 };
        await repo.CompleteAsync(queryId, reportData, stoppingToken);
    }
}
