using Kip.Report.Domain.Exceptions;
using Kip.Report.Domain.Models;
using Kip.Report.Domain.Repositories;
using Microsoft.Extensions.Options;

namespace Kip.Report.Application;

public class QueryService
{
    private readonly IQueryRepository queryRepository;
    private readonly TimeProvider timeProvider;
    private readonly QueryOptions options;

    public QueryService(IQueryRepository queryRepository,
        IOptions<QueryOptions> options,
        TimeProvider timeProvider)
    {
        this.queryRepository = queryRepository;
        this.timeProvider = timeProvider;
        this.options = options.Value;
    }

    public async Task<Guid> AddAsync(Query query, CancellationToken cancellationToken)
    {
        query = query with { Id = Guid.NewGuid(), CreatedAt = timeProvider.GetUtcNow() };
        return await queryRepository.AddAsync(query, cancellationToken);
    }

    public async Task<Query> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        Query? query = await queryRepository.GetAsync(id, cancellationToken);

        if (query is null)
        {
            throw new NotFoundException($"Query with Id {id} not found.");
        }

        TimeSpan timeSpan = timeProvider.GetUtcNow() - query.CreatedAt;

        double elapsedMs = timeSpan.TotalMilliseconds;

        double progressFactor = Math.Clamp(elapsedMs / options.Timeout, 0.0, 1.0);

        ReportData? reportData = null;

        if (progressFactor >= 1.0)
        {
            reportData = new ReportData { CountSignIn = 12 };
        }

        return query with { Percent = (int)(progressFactor * 100), Report = reportData };
    }
}
