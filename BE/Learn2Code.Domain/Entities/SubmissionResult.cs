using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Learn2Code.Domain.Entities;

[Table("submission_results")]
public class SubmissionResult
{
    [Key]
    [Column("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Column("submission_id")]
    public int SubmissionId { get; set; }

    [Column("test_case_id")]
    public int TestCaseId { get; set; }

    [Column("actual_output")]
    public string? ActualOutput { get; set; }

    [Column("is_passed")]
    public bool? IsPassed { get; set; }

    [Column("runtime_ms")]
    public int? RuntimeMs { get; set; }

    // Navigation properties
    [ForeignKey("SubmissionId")]
    public virtual Submission Submission { get; set; } = null!;

    [ForeignKey("TestCaseId")]
    public virtual TestCase TestCase { get; set; } = null!;
}
