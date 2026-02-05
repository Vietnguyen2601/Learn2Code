using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Learn2Code.Domain.Entities;

[Table("final_tests")]
public class FinalTest
{
    [Key]
    [Column("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Column("course_id")]
    public int CourseId { get; set; }

    [Column("title")]
    public string? Title { get; set; }

    [Column("description")]
    public string? Description { get; set; }

    [Column("duration_minutes")]
    public int? DurationMinutes { get; set; }

    [Column("passing_score")]
    public decimal? PassingScore { get; set; }

    // Navigation properties
    [ForeignKey("CourseId")]
    public virtual Course Course { get; set; } = null!;
}
