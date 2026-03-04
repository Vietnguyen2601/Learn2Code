using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Learn2Code.Domain.Entities;

[Table("exercise_progress")]
public class ExerciseProgress
{
    [Key]
    [Column("exprogress_id")]
    public Guid ExProgressId { get; set; }

    [Column("student_id")]
    public Guid StudentId { get; set; }

    [Column("exercise_id")]
    public Guid ExerciseId { get; set; }

    [Column("is_completed")]
    public bool IsCompleted { get; set; } = false;

    [Column("is_passed")]
    public bool IsPassed { get; set; } = false;

    [Column("last_code")]
    public string? LastCode { get; set; }

    [Column("completed_at")]
    public DateTime? CompletedAt { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    [ForeignKey("StudentId")]
    public virtual Account Student { get; set; } = null!;

    [ForeignKey("ExerciseId")]
    public virtual Exercise Exercise { get; set; } = null!;
}
