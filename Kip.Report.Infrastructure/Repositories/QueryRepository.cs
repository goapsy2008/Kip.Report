using AutoMapper;
using AutoMapper.QueryableExtensions;
using Kip.Report.Domain.Models;
using Kip.Report.Domain.Repositories;
using Kip.Report.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace Kip.Report.Infrastructure.Repositories;

public class QueryRepository : IQueryRepository
{
    private readonly IMapper mapper;
    private readonly ApplicationDbContext dbContext;

    public QueryRepository(IMapper mapper, ApplicationDbContext dbContext)
    {
        this.mapper = mapper;
        this.dbContext = dbContext;
    }

    public async Task<Guid> AddAsync(Query query, CancellationToken cancellationToken)
    {
        var dbo = mapper.Map<ReportQueryDbo>(query);
        
        await dbContext.Reports.AddAsync(dbo, cancellationToken);
        
        await dbContext.SaveChangesAsync(cancellationToken);
        
        return dbo.Id;
    }

    public async Task<Query?> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        return await dbContext.Reports
            .Where(r => r.Id == id)
            .ProjectTo<Query>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
