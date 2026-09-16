using System.Text.Json.Serialization;

namespace Kip.Report.Api.Models;

public record QueryResponse
{
    [JsonPropertyName("query")]
    public Guid Id { get; init; }

    [JsonPropertyName("percent")]
    public int Percent { get; init; }

    [JsonPropertyName("result")]
    public ReportDto? Report { get; init; }
}

public record ReportDto
{
    [JsonPropertyName("user_id")] 
    public Guid UserId { get; init; }
    
    [JsonPropertyName("count_sign_in")]
    public int CountSignIn { get; init; }
}