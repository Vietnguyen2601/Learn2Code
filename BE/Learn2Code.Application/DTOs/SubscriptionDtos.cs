using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Learn2Code.Application.DTOs;

// ─── Subscription Package ────────────────────────────────────────────────────

public class SubscriptionPackageDto
{
    [JsonPropertyName("package_id")]
    public Guid PackageId { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("duration_months")]
    public int DurationMonths { get; set; }

    [JsonPropertyName("price")]
    public decimal Price { get; set; }

    [JsonPropertyName("discount_percent")]
    public decimal DiscountPercent { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("is_active")]
    public bool IsActive { get; set; }

    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; set; }

    [JsonPropertyName("updated_at")]
    public DateTime UpdatedAt { get; set; }
}

public class CreateSubscriptionPackageRequest
{
    [Required]
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [Required]
    [Range(1, 120)]
    [JsonPropertyName("duration_months")]
    public int DurationMonths { get; set; }

    [Required]
    [Range(0, double.MaxValue)]
    [JsonPropertyName("price")]
    public decimal Price { get; set; }

    [Range(0, 100)]
    [JsonPropertyName("discount_percent")]
    public decimal DiscountPercent { get; set; } = 0;

    [JsonPropertyName("description")]
    public string? Description { get; set; }
}

public class UpdateSubscriptionPackageRequest
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [Range(1, 120)]
    [JsonPropertyName("duration_months")]
    public int? DurationMonths { get; set; }

    [Range(0, double.MaxValue)]
    [JsonPropertyName("price")]
    public decimal? Price { get; set; }

    [Range(0, 100)]
    [JsonPropertyName("discount_percent")]
    public decimal? DiscountPercent { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("is_active")]
    public bool? IsActive { get; set; }
}

// ─── User Subscription ───────────────────────────────────────────────────────

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

public class CreateSubscriptionRequest
{
    [Required]
    [JsonPropertyName("package_id")]
    public Guid PackageId { get; set; }
}

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
