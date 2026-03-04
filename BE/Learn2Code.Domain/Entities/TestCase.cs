using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Learn2Code.Domain.Entities;

[Table("test_cases")]
public class TestCase
{
    [Key]
    [Column("testcase_id")]
    public Guid TestCaseId { get; set; }

    [Column("exercise_id")]
    public Guid ExerciseId { get; set; }

    [Required]
    [Column("expected_output")]
    public string ExpectedOutput { get; set; } = string.Empty;

    [Column("is_hidden")]
    public bool IsHidden { get; set; } = false;

    [Column("weight")]
    public decimal Weight { get; set; } = 1;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    [ForeignKey("ExerciseId")]
    public virtual Exercise Exercise { get; set; } = null!;
}
