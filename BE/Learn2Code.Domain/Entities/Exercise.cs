using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Learn2Code.Domain.Enums;

namespace Learn2Code.Domain.Entities;

[Table("exercises")]
public class Exercise
{
    [Key]
    [Column("exercise_id")]
    public Guid ExerciseId { get; set; }

    [Column("lesson_id")]
    public Guid LessonId { get; set; }

    [Column("order_number")]
    public int OrderNumber { get; set; }

    [Column("exercise_type")]
    public ExerciseType ExerciseType { get; set; }

    [Required]
    [Column("narrative")]
    public string Narrative { get; set; } = string.Empty;

    [Column("language")]
    public string? Language { get; set; }

    [Column("starter_code")]
    public string? StarterCode { get; set; }

    [Column("solution_code")]
    public string? SolutionCode { get; set; }

    [Column("instruction")]
    public string? Instruction { get; set; }

    [Column("hint")]
    public string? Hint { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    [ForeignKey("LessonId")]
    public virtual Lesson Lesson { get; set; } = null!;

    public virtual ICollection<TestCase> TestCases { get; set; } = new List<TestCase>();
    public virtual ICollection<ExerciseMedia> ExerciseMedias { get; set; } = new List<ExerciseMedia>();
    public virtual ICollection<ExerciseProgress> ExerciseProgresses { get; set; } = new List<ExerciseProgress>();
}
