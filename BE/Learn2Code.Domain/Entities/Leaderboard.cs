using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Learn2Code.Domain.Entities;

[Table("leaderboards")]
public class Leaderboard
{
    [Key]
    [Column("leaderboard_id")]
    public Guid LeaderboardId { get; set; }

    [Column("course_id")]
    public Guid CourseId { get; set; }

    [Column("student_id")]
    public Guid StudentId { get; set; }

    [Column("total_score")]
    public decimal TotalScore { get; set; } = 0;

    [Column("rank")]
    public int? Rank { get; set; }

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    [ForeignKey("CourseId")]
    public virtual Course Course { get; set; } = null!;

    [ForeignKey("StudentId")]
    public virtual Account Student { get; set; } = null!;
}
