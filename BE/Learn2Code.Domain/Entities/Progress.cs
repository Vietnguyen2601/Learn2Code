using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Learn2Code.Domain.Enums;

namespace Learn2Code.Domain.Entities;

[Table("progresses")]
public class Progress
{
    [Key]
    [Column("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Column("student_id")]
    public Guid StudentId { get; set; }

    [Column("lesson_id")]
    public int LessonId { get; set; }

    [Column("status")]
    public ProgressStatus Status { get; set; } = ProgressStatus.NotStarted;

    [Column("last_accessed_at")]
    public DateTime? LastAccessedAt { get; set; }

    [Column("completed_at")]
    public DateTime? CompletedAt { get; set; }

    // Navigation properties
    [ForeignKey("StudentId")]
    public virtual Account Student { get; set; } = null!;

    [ForeignKey("LessonId")]
    public virtual Lesson Lesson { get; set; } = null!;
}
