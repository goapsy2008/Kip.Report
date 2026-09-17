using Kip.Report.Domain.Models;
using NpgsqlTypes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kip.Report.Infrastructure.Models;

[Table("report_queries")]
public class ReportQueryDbo
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("user_id")]
    public Guid UserId { get; set; }

    [Column("period", TypeName = "tstzrange")]
    public NpgsqlRange<DateTimeOffset> Period { get; set; }

    [Column("created_at", TypeName = "timestamp with time zone")]
    public DateTimeOffset CreatedAt { get; set; }

    [Column("started_at", TypeName = "timestamp with time zone")]
    public DateTimeOffset? StartedAt { get; set; }

    [Column("is_completed")]
    public bool IsCompleted { get; set; } = false;

    [Column("report_json", TypeName = "jsonb")]
    public ReportData? Report { get; set; }
}
