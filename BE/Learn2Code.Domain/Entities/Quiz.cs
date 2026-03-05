using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Learn2Code.Domain.Entities;

[Table("quizzes")]
public class Quiz
{
    [Key]
    [Column("quiz_id")]
    public Guid QuizId { get; set; }

    [Column("lesson_id")]
    public Guid LessonId { get; set; }

    [Column("order_number")]
    public int OrderNumber { get; set; }

    [Required]
    [Column("question")]
    public string Question { get; set; } = string.Empty;

    [Column("explanation")]
    public string? Explanation { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    [ForeignKey("LessonId")]
    public virtual Lesson Lesson { get; set; } = null!;

    public virtual ICollection<QuizOption> Options { get; set; } = new List<QuizOption>();
    public virtual ICollection<SectionQuizAnswer> SectionQuizAnswers { get; set; } = new List<SectionQuizAnswer>();
}
