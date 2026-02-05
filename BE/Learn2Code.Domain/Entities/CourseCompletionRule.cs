using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Learn2Code.Domain.Entities;

[Table("course_completion_rules")]
public class CourseCompletionRule
{
    [Key]
    [Column("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Column("course_id")]
    public int CourseId { get; set; }

    [Column("min_lesson_completion_percent")]
    public decimal? MinLessonCompletionPercent { get; set; }

    [Column("min_exercise_pass_percent")]
    public decimal? MinExercisePassPercent { get; set; }

    [Column("min_quiz_score")]
    public decimal? MinQuizScore { get; set; }

    [Column("require_final_test")]
    public bool? RequireFinalTest { get; set; }

    // Navigation properties
    [ForeignKey("CourseId")]
    public virtual Course Course { get; set; } = null!;
}
