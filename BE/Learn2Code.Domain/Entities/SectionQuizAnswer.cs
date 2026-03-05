using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Learn2Code.Domain.Entities;

[Table("section_quiz_answers")]
public class SectionQuizAnswer
{
    [Key]
    [Column("answer_id")]
    public Guid AnswerId { get; set; }

    [Column("attempt_id")]
    public Guid AttemptId { get; set; }

    [Column("quiz_id")]
    public Guid QuizId { get; set; }

    [Column("option_id")]
    public Guid OptionId { get; set; }

    [Column("is_correct")]
    public bool IsCorrect { get; set; }

    // Navigation properties
    [ForeignKey("AttemptId")]
    public virtual SectionQuizAttempt Attempt { get; set; } = null!;

    [ForeignKey("QuizId")]
    public virtual Quiz Quiz { get; set; } = null!;

    [ForeignKey("OptionId")]
    public virtual QuizOption Option { get; set; } = null!;
}
