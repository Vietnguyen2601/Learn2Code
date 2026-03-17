using System.Text.Json.Serialization;

namespace Learn2Code.Application.DTOs.PaymentDTOs.PaymentRequests;

/// <summary>
/// Webhook payload from PayOS
/// </summary>
public class PayOsWebhookRequest
{
    [JsonPropertyName("code")]
    public string Code { get; set; } = string.Empty;

    [JsonPropertyName("desc")]
    public string Desc { get; set; } = string.Empty;

    [JsonPropertyName("data")]
    public PayOsWebhookData? Data { get; set; }

    [JsonPropertyName("signature")]
    public string? Signature { get; set; }
}
