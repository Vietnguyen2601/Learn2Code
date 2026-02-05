using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Learn2Code.Domain.Enums;

namespace Learn2Code.Domain.Entities;

[Table("enrollments")]
public class Enrollment
{
    [Key]
    [Column("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Column("student_id")]
    public Guid StudentId { get; set; }

    [Column("course_id")]
    public int CourseId { get; set; }

    [Column("status")]
    public EnrollmentStatus Status { get; set; } = EnrollmentStatus.Enrolled;

    [Column("progress_percentage")]
    public decimal ProgressPercentage { get; set; } = 0;

    [Column("enrollment_date")]
    public DateTime? EnrollmentDate { get; set; }

    [Column("completed_at")]
    public DateTime? CompletedAt { get; set; }

    // Navigation properties
    [ForeignKey("StudentId")]
    public virtual Account Student { get; set; } = null!;

    [ForeignKey("CourseId")]
    public virtual Course Course { get; set; } = null!;

    public virtual Payment? Payment { get; set; }
}
