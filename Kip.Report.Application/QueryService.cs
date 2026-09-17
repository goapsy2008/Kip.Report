using Kip.Report.Domain.Exceptions;
using Kip.Report.Domain.Models;
using Kip.Report.Domain.Repositories;
using Microsoft.Extensions.Options;

namespace Kip.Report.Application;

public class QueryService
{
    private readonly IQueryRepository queryRepository;
    private readonly BackgroundReportQueue queue;
    private readonly TimeProvider timeProvider;
    private readonly QueryOptions options;

    public QueryService(IQueryRepository queryRepository,
        BackgroundReportQueue queue,
        IOptions<QueryOptions> options,
        TimeProvider timeProvider)
    {
        this.queryRepository = queryRepository;
        this.queue = queue;
        this.timeProvider = timeProvider;
        this.options = options.Value;
    }

    public async Task<Guid> AddAsync(Query query, CancellationToken cancellationToken)
    {
        query = query with
        {
            Id = Guid.NewGuid(),
            CreatedAt = timeProvider.GetUtcNow(),
            IsCompleted = false,
            StartedAt = null
        };

        await queryRepository.AddAsync(query, cancellationToken);
        await queue.QueueReportAsync(query.Id, cancellationToken);

        return query.Id;
    }

    public async Task<Query> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        Query? query = await queryRepository.GetAsync(id, cancellationToken);

        if (query is null)
        {
            throw new NotFoundException($"Query with Id {id} not found.");
        }

        if (query.IsCompleted)
        {
            return query with { Percent = 100 };
        }

        if (query.StartedAt is null)
        {
            return query with { Percent = 0, Report = null };
        }

        TimeSpan timeSpan = timeProvider.GetUtcNow() - query.StartedAt.Value;

        double elapsedMs = timeSpan.TotalMilliseconds;

        double progressFactor = Math.Clamp(elapsedMs / options.Timeout, 0.0, 1.0);

        return query with { Percent = (int)(progressFactor * 100), Report = null };
    }
}
