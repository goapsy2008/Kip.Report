using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Kip.Report.Api.Models;

public record QueryRequest : IValidatableObject
{
    [Required]
    [JsonPropertyName("user_id")]
    public Guid? UserId { get; init; }

    [Required]
    [JsonPropertyName("from")]
    public DateTimeOffset? From { get; init; }

    [Required]
    [JsonPropertyName("to")]
    public DateTimeOffset? To { get; init; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (From!.Value > To!.Value)
        {
            yield return new ValidationResult("Некорректный период.");
        }
    }
}