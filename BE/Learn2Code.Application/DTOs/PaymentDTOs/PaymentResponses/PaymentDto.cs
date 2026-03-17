using System.Text.Json.Serialization;

namespace Learn2Code.Application.DTOs.PaymentDTOs.PaymentResponses;

public class PaymentDto
{
    [JsonPropertyName("payment_id")]
    public Guid PaymentId { get; set; }

    [JsonPropertyName("subscription_id")]
    public Guid SubscriptionId { get; set; }

    [JsonPropertyName("amount")]
    public decimal Amount { get; set; }

    [JsonPropertyName("payment_method")]
    public string PaymentMethod { get; set; } = string.Empty;

    [JsonPropertyName("transaction_id")]
    public string? TransactionId { get; set; }

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("paid_at")]
    public DateTime? PaidAt { get; set; }

    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; set; }
}
