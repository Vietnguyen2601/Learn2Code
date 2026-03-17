using System.Text.Json.Serialization;

namespace Learn2Code.Application.DTOs.SubscriptionDTOs.SubscriptionResponses;

public class SubscriptionDto
{
    [JsonPropertyName("subscription_id")]
    public Guid SubscriptionId { get; set; }

    [JsonPropertyName("user_id")]
    public Guid UserId { get; set; }

    [JsonPropertyName("package")]
    public SubscriptionPackageDto? Package { get; set; }

    [JsonPropertyName("start_date")]
    public DateTime StartDate { get; set; }

    [JsonPropertyName("end_date")]
    public DateTime EndDate { get; set; }

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("renewed_from_id")]
    public Guid? RenewedFromId { get; set; }

    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; set; }

    [JsonPropertyName("updated_at")]
    public DateTime UpdatedAt { get; set; }
}
