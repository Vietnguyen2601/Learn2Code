using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Learn2Code.Domain.Entities;

[Table("feedbacks")]
public class Feedback
{
    [Key]
    [Column("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Column("course_id")]
    public int CourseId { get; set; }

    [Column("student_id")]
    public Guid StudentId { get; set; }

    [Column("rating")]
    public int? Rating { get; set; }

    [Column("comment")]
    public string? Comment { get; set; }

    [Column("created_at")]
    public DateTime? CreatedAt { get; set; }

    // Navigation properties
    [ForeignKey("CourseId")]
    public virtual Course Course { get; set; } = null!;

    [ForeignKey("StudentId")]
    public virtual Account Student { get; set; } = null!;
}
