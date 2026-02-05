using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Learn2Code.Domain.Entities;

[Table("test_cases")]
public class TestCase
{
    [Key]
    [Column("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Column("exercise_id")]
    public int ExerciseId { get; set; }

    [Column("input")]
    public string? Input { get; set; }

    [Column("expected_output")]
    public string? ExpectedOutput { get; set; }

    [Column("is_hidden")]
    public bool? IsHidden { get; set; }

    [Column("weight")]
    public decimal? Weight { get; set; }

    // Navigation properties
    [ForeignKey("ExerciseId")]
    public virtual Exercise Exercise { get; set; } = null!;

    public virtual ICollection<SubmissionResult> SubmissionResults { get; set; } = new List<SubmissionResult>();
}
