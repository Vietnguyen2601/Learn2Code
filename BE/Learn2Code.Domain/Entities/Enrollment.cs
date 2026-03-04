using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Learn2Code.Domain.Enums;

namespace Learn2Code.Domain.Entities;

[Table("enrollments")]
public class Enrollment
{
    [Key]
    [Column("enrollment_id")]
    public Guid EnrollmentId { get; set; }

    [Column("student_id")]
    public Guid StudentId { get; set; }

    [Column("course_id")]
    public Guid CourseId { get; set; }

    [Column("status")]
    public EnrollmentStatus Status { get; set; } = EnrollmentStatus.Enrolled;

    [Column("progress_pct")]
    public decimal ProgressPct { get; set; } = 0;

    [Column("enrolled_at")]
    public DateTime EnrolledAt { get; set; } = DateTime.UtcNow;

    [Column("activated_at")]
    public DateTime? ActivatedAt { get; set; }

    [Column("completed_at")]
    public DateTime? CompletedAt { get; set; }

    [Column("subscription_id")]
    public Guid? SubscriptionId { get; set; }

    // Navigation properties
    [ForeignKey("StudentId")]
    public virtual Account Student { get; set; } = null!;

    [ForeignKey("CourseId")]
    public virtual Course Course { get; set; } = null!;

    [ForeignKey("SubscriptionId")]
    public virtual UserSubscription? Subscription { get; set; }
}
