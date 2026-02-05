using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Learn2Code.Domain.Enums;

namespace Learn2Code.Domain.Entities;

[Table("exercises")]
public class Exercise
{
    [Key]
    [Column("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Column("lesson_id")]
    public int LessonId { get; set; }

    [Column("title")]
    public string? Title { get; set; }

    [Column("description")]
    public string? Description { get; set; }

    [Column("difficulty")]
    public ExerciseDifficulty? Difficulty { get; set; }

    [Column("language")]
    public ProgrammingLanguage? Language { get; set; }

    [Column("starter_code")]
    public string? StarterCode { get; set; }

    [Column("solution_code")]
    public string? SolutionCode { get; set; }

    [Column("created_at")]
    public DateTime? CreatedAt { get; set; }

    // Navigation properties
    [ForeignKey("LessonId")]
    public virtual Lesson Lesson { get; set; } = null!;

    public virtual ICollection<TestCase> TestCases { get; set; } = new List<TestCase>();
    public virtual ICollection<Submission> Submissions { get; set; } = new List<Submission>();
}
