using System.Text.Json.Serialization;

namespace Learn2Code.Application.DTOs.SubscriptionDTOs.SubscriptionResponses;

/// <summary>
/// Returned after POST /subscriptions or POST /subscriptions/:id/renew.
/// The client should redirect the user to payment_url to complete payment.
/// </summary>
public class CreateSubscriptionResponse
{
    [JsonPropertyName("subscription_id")]
    public Guid SubscriptionId { get; set; }

    [JsonPropertyName("payment_id")]
    public Guid PaymentId { get; set; }

    [JsonPropertyName("package_name")]
    public string PackageName { get; set; } = string.Empty;

    [JsonPropertyName("amount")]
    public decimal Amount { get; set; }

    [JsonPropertyName("payment_method")]
    public string PaymentMethod { get; set; } = string.Empty;

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("payment_url")]
    public string PaymentUrl { get; set; } = string.Empty;

    [JsonPropertyName("expired_at")]
    public DateTime ExpiredAt { get; set; }
}
