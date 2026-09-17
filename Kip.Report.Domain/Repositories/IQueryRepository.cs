using Kip.Report.Domain.Models;

namespace Kip.Report.Domain.Repositories;

public interface IQueryRepository
{
    Task<Guid> AddAsync(Query query, CancellationToken cancellationToken);

    Task<Query?> GetAsync(Guid id, CancellationToken cancellationToken);

    Task<List<Guid>> GetUncompletedAsync(CancellationToken cancellationToken);

    Task ResetStartAsync(Guid id, CancellationToken cancellationToken);

    Task CompleteAsync(Guid id, ReportData reportData, CancellationToken cancellationToken);

    Task StartAsync(Guid id, DateTimeOffset startedAt, CancellationToken cancellationToken);
}