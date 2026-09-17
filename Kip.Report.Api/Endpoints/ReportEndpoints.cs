using AutoMapper;
using Kip.Report.Api.Models;
using Kip.Report.Application;
using Kip.Report.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace Kip.Report.Api.Endpoints;

public static class ReportEndpoints
{
    public static IEndpointRouteBuilder MapReportEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/report")
                       .WithTags("Reports");

        group.MapGet("/info", GetReportInfoAsync);
        group.MapPost("/user_statistics", GetUserStatisticsAsync).WithValidation<QueryRequest>();

        return app;
    }

    private static async Task<IResult> GetReportInfoAsync(
        [FromQuery(Name = "query")] Guid id,
        IMapper mapper,
        QueryService queryService,
        CancellationToken cancellationToken)
    {
        var query = await queryService.GetAsync(id, cancellationToken);

        var response = mapper.Map<QueryResponse>(query);

        return Results.Ok(response);
    }

    private static async Task<IResult> GetUserStatisticsAsync(
        QueryRequest request,
        IMapper mapper,
        QueryService queryService,
        CancellationToken cancellationToken)
    {
        var query = mapper.Map<Query>(request);

        var resultId = await queryService.AddAsync(query, cancellationToken);

        return Results.Accepted(uri: null, resultId);
    }
}
