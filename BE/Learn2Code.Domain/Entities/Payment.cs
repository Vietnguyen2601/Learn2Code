using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Learn2Code.Domain.Enums;

namespace Learn2Code.Domain.Entities;

[Table("payments")]
public class Payment
{
    [Key]
    [Column("payment_id")]
    public Guid PaymentId { get; set; }

    [Column("subscription_id")]
    public Guid SubscriptionId { get; set; }

    [Column("amount")]
    public decimal Amount { get; set; }

    [Column("payment_method")]
    public PaymentMethod PaymentMethod { get; set; }

    [Column("transaction_id")]
    public string? TransactionId { get; set; }

    [Column("status")]
    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;

    [Column("paid_at")]
    public DateTime? PaidAt { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    [ForeignKey("SubscriptionId")]
    public virtual UserSubscription Subscription { get; set; } = null!;
}
