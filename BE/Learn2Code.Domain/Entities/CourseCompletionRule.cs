using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Learn2Code.Domain.Entities;

[Table("course_completion_rules")]
public class CourseCompletionRule
{
    [Key]
    [Column("rule_id")]
    public Guid RuleId { get; set; }

    [Column("course_id")]
    public Guid CourseId { get; set; }

    [Column("min_weight_score")]
    public decimal MinWeightScore { get; set; } = 0;

    [Column("require_all_section_quiz")]
    public bool RequireAllSectionQuiz { get; set; } = true;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    [ForeignKey("CourseId")]
    public virtual Course Course { get; set; } = null!;
}
