using System.Text.Json.Serialization;

namespace Learn2Code.Application.DTOs.PaymentDTOs.PaymentRequests;

public class PayOsWebhookData
{
    [JsonPropertyName("orderCode")]
    public long OrderCode { get; set; }

    [JsonPropertyName("amount")]
    public int Amount { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("accountNumber")]
    public string? AccountNumber { get; set; }

    [JsonPropertyName("reference")]
    public string? Reference { get; set; }

    [JsonPropertyName("transactionDateTime")]
    public string? TransactionDateTime { get; set; }

    [JsonPropertyName("currency")]
    public string Currency { get; set; } = "VND";

    [JsonPropertyName("paymentLinkId")]
    public string? PaymentLinkId { get; set; }

    [JsonPropertyName("code")]
    public string Code { get; set; } = string.Empty;

    [JsonPropertyName("desc")]
    public string Desc { get; set; } = string.Empty;

    [JsonPropertyName("counterAccountBankId")]
    public string? CounterAccountBankId { get; set; }

    [JsonPropertyName("counterAccountBankName")]
    public string? CounterAccountBankName { get; set; }

    [JsonPropertyName("counterAccountName")]
    public string? CounterAccountName { get; set; }

    [JsonPropertyName("counterAccountNumber")]
    public string? CounterAccountNumber { get; set; }

    [JsonPropertyName("virtualAccountName")]
    public string? VirtualAccountName { get; set; }

    [JsonPropertyName("virtualAccountNumber")]
    public string? VirtualAccountNumber { get; set; }
}
