using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Learn2Code.Application.DTOs;

// ─── Payment History ─────────────────────────────────────────────────────────

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

// ─── PayOS Webhook DTOs (used in API layer) ──────────────────────────────────

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
