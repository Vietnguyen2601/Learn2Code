using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Learn2Code.Domain.Entities;

[Table("section_quiz_attempts")]
public class SectionQuizAttempt
{
    [Key]
    [Column("attempt_id")]
    public Guid AttemptId { get; set; }

    [Column("section_id")]
    public Guid SectionId { get; set; }

    [Column("student_id")]
    public Guid StudentId { get; set; }

    [Column("score")]
    public decimal Score { get; set; }

    [Column("is_passed")]
    public bool IsPassed { get; set; }

    [Column("attempted_at")]
    public DateTime AttemptedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    [ForeignKey("SectionId")]
    public virtual Section Section { get; set; } = null!;

    [ForeignKey("StudentId")]
    public virtual Account Student { get; set; } = null!;

    public virtual ICollection<SectionQuizAnswer> Answers { get; set; } = new List<SectionQuizAnswer>();
}
