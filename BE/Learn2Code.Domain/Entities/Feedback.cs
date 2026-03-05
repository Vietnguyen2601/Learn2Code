using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Learn2Code.Domain.Entities;

[Table("feedbacks")]
public class Feedback
{
    [Key]
    [Column("feedback_id")]
    public Guid FeedbackId { get; set; }

    [Column("course_id")]
    public Guid CourseId { get; set; }

    [Column("student_id")]
    public Guid StudentId { get; set; }

    [Column("rating")]
    public int Rating { get; set; }

    [Column("comment")]
    public string? Comment { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    [ForeignKey("CourseId")]
    public virtual Course Course { get; set; } = null!;

    [ForeignKey("StudentId")]
    public virtual Account Student { get; set; } = null!;
}
