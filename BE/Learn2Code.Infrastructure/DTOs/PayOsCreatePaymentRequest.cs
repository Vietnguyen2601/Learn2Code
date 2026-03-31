using System.Text.Json.Serialization;

namespace Learn2Code.Infrastructure.DTOs;

/// <summary>
/// Request to create PayOS payment link
/// </summary>
public class PayOsCreatePaymentRequest
{
    [JsonPropertyName("orderCode")]
    public long OrderCode { get; set; }

    [JsonPropertyName("amount")]
    public int Amount { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("returnUrl")]
    public string ReturnUrl { get; set; } = string.Empty;

    [JsonPropertyName("cancelUrl")]
    public string CancelUrl { get; set; } = string.Empty;

    [JsonPropertyName("webhookUrl")]
    public string? WebhookUrl { get; set; }

    [JsonPropertyName("signature")]
    public string? Signature { get; set; }
}
