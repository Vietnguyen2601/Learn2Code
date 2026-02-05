using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Learn2Code.Domain.Entities;

[Table("quiz_options")]
public class QuizOption
{
    [Key]
    [Column("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Column("quiz_id")]
    public int QuizId { get; set; }

    [Column("content")]
    public string? Content { get; set; }

    [Column("is_correct")]
    public bool? IsCorrect { get; set; }

    // Navigation properties
    [ForeignKey("QuizId")]
    public virtual Quiz Quiz { get; set; } = null!;

    public virtual ICollection<QuizSubmission> QuizSubmissions { get; set; } = new List<QuizSubmission>();
}
