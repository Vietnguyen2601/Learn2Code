using System.Text.Json.Serialization;

namespace Learn2Code.Infrastructure.DTOs;

public class PayOsPaymentData
{
    [JsonPropertyName("bin")]
    public string? Bin { get; set; }

    [JsonPropertyName("accountNumber")]
    public string? AccountNumber { get; set; }

    [JsonPropertyName("accountName")]
    public string? AccountName { get; set; }

    [JsonPropertyName("amount")]
    public int Amount { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("orderCode")]
    public long OrderCode { get; set; }

    [JsonPropertyName("currency")]
    public string Currency { get; set; } = "VND";

    [JsonPropertyName("paymentLinkId")]
    public string? PaymentLinkId { get; set; }

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("checkoutUrl")]
    public string? CheckoutUrl { get; set; }

    [JsonPropertyName("qrCode")]
    public string? QrCode { get; set; }
}
