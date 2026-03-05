using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Learn2Code.Domain.Entities;

[Table("lessons")]
public class Lesson
{
    [Key]
    [Column("lesson_id")]
    public Guid LessonId { get; set; }

    [Column("section_id")]
    public Guid SectionId { get; set; }

    [Required]
    [Column("title")]
    public string Title { get; set; } = string.Empty;

    [Column("order_number")]
    public int OrderNumber { get; set; }

    [Column("is_free_preview")]
    public bool IsFreePreview { get; set; } = false;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    [ForeignKey("SectionId")]
    public virtual Section Section { get; set; } = null!;

    public virtual ICollection<Exercise> Exercises { get; set; } = new List<Exercise>();
    public virtual ICollection<Quiz> Quizzes { get; set; } = new List<Quiz>();
    public virtual ICollection<LessonProgress> LessonProgresses { get; set; } = new List<LessonProgress>();
}
