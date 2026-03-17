using System.Text.Json.Serialization;

namespace Learn2Code.Application.DTOs.PaymentDTOs.PaymentResponses;

/// <summary>
/// Response for webhook processing
/// </summary>
public class PayOsWebhookResponse
{
    [JsonPropertyName("success")]
    public bool Success { get; set; }

    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;

    [JsonPropertyName("new_status")]
    public string? NewStatus { get; set; }
}
