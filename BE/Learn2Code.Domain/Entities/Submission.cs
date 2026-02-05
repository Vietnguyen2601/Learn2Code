using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Learn2Code.Domain.Enums;

namespace Learn2Code.Domain.Entities;

[Table("submissions")]
public class Submission
{
    [Key]
    [Column("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Column("exercise_id")]
    public int ExerciseId { get; set; }

    [Column("student_id")]
    public Guid StudentId { get; set; }

    [Column("submitted_code")]
    public string? SubmittedCode { get; set; }

    [Column("attempt_number")]
    public int? AttemptNumber { get; set; }

    [Column("status")]
    public SubmissionStatus Status { get; set; } = SubmissionStatus.Pending;

    [Column("submitted_at")]
    public DateTime? SubmittedAt { get; set; }

    // Navigation properties
    [ForeignKey("ExerciseId")]
    public virtual Exercise Exercise { get; set; } = null!;

    [ForeignKey("StudentId")]
    public virtual Account Student { get; set; } = null!;

    public virtual ICollection<SubmissionResult> Results { get; set; } = new List<SubmissionResult>();
}
