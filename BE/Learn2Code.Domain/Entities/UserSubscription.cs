using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Learn2Code.Domain.Enums;

namespace Learn2Code.Domain.Entities;

[Table("user_subscriptions")]
public class UserSubscription
{
    [Key]
    [Column("subscription_id")]
    public Guid SubscriptionId { get; set; }

    [Column("user_id")]
    public Guid UserId { get; set; }

    [Column("package_id")]
    public Guid PackageId { get; set; }

    [Column("start_date")]
    public DateTime StartDate { get; set; }

    [Column("end_date")]
    public DateTime EndDate { get; set; }

    [Column("status")]
    public SubscriptionStatus Status { get; set; } = SubscriptionStatus.Pending;

    [Column("renewed_from_id")]
    public Guid? RenewedFromId { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    [ForeignKey("UserId")]
    public virtual Account User { get; set; } = null!;

    [ForeignKey("PackageId")]
    public virtual SubscriptionPackage Package { get; set; } = null!;

    [ForeignKey("RenewedFromId")]
    public virtual UserSubscription? RenewedFrom { get; set; }

    public virtual ICollection<UserSubscription> RenewedSubscriptions { get; set; } = new List<UserSubscription>();
    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
    public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
}
