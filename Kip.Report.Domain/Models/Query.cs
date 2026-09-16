namespace Kip.Report.Domain.Models;

public record Query
{
    public Guid Id { get; init; }

    public Guid UserId { get; init; }

    public DateTimeOffset From { get; init; }

    public DateTimeOffset To { get; init; }

    public int Percent { get; init; }

    public DateTimeOffset CreatedAt { get; init; }

    public ReportData? Report { get; init; }
}
