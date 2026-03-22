using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Learn2Code.Application.DTOs.SubscriptionDTOs.SubscriptionRequests;

public class UpdateSubscriptionPackageRequest
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [Range(1, 120)]
    [JsonPropertyName("duration_months")]
    public int? DurationMonths { get; set; }

    [Range(1000, double.MaxValue)]
    [JsonPropertyName("price")]
    public decimal? Price { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("is_active")]
    public bool? IsActive { get; set; }
}
