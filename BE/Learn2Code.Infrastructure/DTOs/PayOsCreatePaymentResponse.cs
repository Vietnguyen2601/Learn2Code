using System.Text.Json.Serialization;

namespace Learn2Code.Infrastructure.DTOs;

/// <summary>
/// Response from PayOS after creating payment link
/// </summary>
public class PayOsCreatePaymentResponse
{
    [JsonPropertyName("code")]
    public string Code { get; set; } = string.Empty;

    [JsonPropertyName("desc")]
    public string Desc { get; set; } = string.Empty;

    [JsonPropertyName("data")]
    public PayOsPaymentData? Data { get; set; }
}
