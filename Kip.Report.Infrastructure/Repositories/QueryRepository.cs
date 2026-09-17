using AutoMapper;
using AutoMapper.QueryableExtensions;
using Kip.Report.Domain.Models;
using Kip.Report.Domain.Repositories;
using Kip.Report.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

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

    public async Task<List<Guid>> GetUncompletedAsync(CancellationToken cancellationToken)
    {
        return await dbContext.Reports
            .Where(r => !r.IsCompleted)
            .Select(r => r.Id)
        .ToListAsync(cancellationToken);
    }

    public async Task ResetStartAsync(Guid id, CancellationToken cancellationToken)
    {
        var dbo = await dbContext.Reports.FindAsync(new object[] { id }, cancellationToken);
        if (dbo != null)
        {
            dbo.StartedAt = null;
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task StartAsync(Guid id, DateTimeOffset startedAt, CancellationToken cancellationToken)
    {
        var dbo = await dbContext.Reports.FindAsync(new object[] { id }, cancellationToken);
        if (dbo != null)
        {
            dbo.StartedAt = startedAt;
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task CompleteAsync(Guid id, ReportData reportData, CancellationToken cancellationToken)
    {
        var dbo = await dbContext.Reports.FindAsync(new object[] { id }, cancellationToken);
        if (dbo != null)
        {
            dbo.Report = reportData;
            dbo.IsCompleted = true;
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
