using Kip.Report.Domain.Models;

namespace Kip.Report.Domain.Repositories;

public interface IQueryRepository
{
    Task<Guid> AddAsync(Query query, CancellationToken cancellationToken);

    Task<Query?> GetAsync(Guid id, CancellationToken cancellationToken);
}