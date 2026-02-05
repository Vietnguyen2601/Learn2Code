using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Learn2Code.Domain.Enums;

namespace Learn2Code.Domain.Entities;

[Table("payments")]
public class Payment
{
    [Key]
    [Column("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Column("enrollment_id")]
    public int EnrollmentId { get; set; }

    [Column("amount")]
    public decimal? Amount { get; set; }

    [Column("payment_method")]
    public PaymentMethod? PaymentMethod { get; set; }

    [Column("transaction_id")]
    public string? TransactionId { get; set; }

    [Column("status")]
    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;

    [Column("paid_at")]
    public DateTime? PaidAt { get; set; }

    // Navigation properties
    [ForeignKey("EnrollmentId")]
    public virtual Enrollment Enrollment { get; set; } = null!;
}
