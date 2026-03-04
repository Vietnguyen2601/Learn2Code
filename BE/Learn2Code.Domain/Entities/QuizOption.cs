using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Learn2Code.Domain.Entities;

[Table("quiz_options")]
public class QuizOption
{
    [Key]
    [Column("option_id")]
    public Guid OptionId { get; set; }

    [Column("quiz_id")]
    public Guid QuizId { get; set; }

    [Required]
    [Column("content")]
    public string Content { get; set; } = string.Empty;

    [Column("is_correct")]
    public bool IsCorrect { get; set; } = false;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    [ForeignKey("QuizId")]
    public virtual Quiz Quiz { get; set; } = null!;

    public virtual ICollection<SectionQuizAnswer> SectionQuizAnswers { get; set; } = new List<SectionQuizAnswer>();
}
