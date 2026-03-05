using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Learn2Code.Domain.Enums;

namespace Learn2Code.Domain.Entities;

[Table("lesson_progress")]
public class LessonProgress
{
    [Key]
    [Column("progress_id")]
    public Guid ProgressId { get; set; }

    [Column("student_id")]
    public Guid StudentId { get; set; }

    [Column("lesson_id")]
    public Guid LessonId { get; set; }

    [Column("status")]
    public LessonProgressStatus Status { get; set; } = LessonProgressStatus.NotStarted;

    [Column("last_accessed_at")]
    public DateTime? LastAccessedAt { get; set; }

    [Column("completed_at")]
    public DateTime? CompletedAt { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    [ForeignKey("StudentId")]
    public virtual Account Student { get; set; } = null!;

    [ForeignKey("LessonId")]
    public virtual Lesson Lesson { get; set; } = null!;
}
