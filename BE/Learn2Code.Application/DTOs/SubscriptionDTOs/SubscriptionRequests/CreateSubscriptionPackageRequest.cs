using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Learn2Code.Application.DTOs.SubscriptionDTOs.SubscriptionRequests;

public class CreateSubscriptionPackageRequest
{
    [Required]
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [Required]
    [Range(1, 120)]
    [JsonPropertyName("duration_months")]
    public int DurationMonths { get; set; }

    [Required]
    [Range(1000, double.MaxValue)]
    [JsonPropertyName("price")]
    public decimal Price { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }
}
