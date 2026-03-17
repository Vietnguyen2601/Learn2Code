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
    [Range(0, double.MaxValue)]
    [JsonPropertyName("price")]
    public decimal Price { get; set; }

    [Range(0, 100)]
    [JsonPropertyName("discount_percent")]
    public decimal DiscountPercent { get; set; } = 0;

    [JsonPropertyName("description")]
    public string? Description { get; set; }
}
