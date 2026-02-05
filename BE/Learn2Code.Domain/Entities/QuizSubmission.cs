using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Learn2Code.Domain.Entities;

[Table("quiz_submissions")]
public class QuizSubmission
{
    [Key]
    [Column("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Column("quiz_id")]
    public int QuizId { get; set; }

    [Column("student_id")]
    public Guid StudentId { get; set; }

    [Column("selected_option_id")]
    public int? SelectedOptionId { get; set; }

    [Column("is_correct")]
    public bool? IsCorrect { get; set; }

    [Column("submitted_at")]
    public DateTime? SubmittedAt { get; set; }

    // Navigation properties
    [ForeignKey("QuizId")]
    public virtual Quiz Quiz { get; set; } = null!;

    [ForeignKey("StudentId")]
    public virtual Account Student { get; set; } = null!;

    [ForeignKey("SelectedOptionId")]
    public virtual QuizOption? SelectedOption { get; set; }
}
