using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Learn2Code.Domain.Entities;

[Table("discussions")]
public class Discussion
{
    [Key]
    [Column("discussion_id")]
    public Guid DiscussionId { get; set; }

    [Column("lesson_id")]
    public Guid LessonId { get; set; }

    [Column("creator_id")]
    public Guid CreatorId { get; set; }

    [Required]
    [Column("title")]
    public string Title { get; set; } = string.Empty;

    [Required]
    [Column("content")]
    public string Content { get; set; } = string.Empty;

    [Column("is_pinned")]
    public bool IsPinned { get; set; } = false;

    [Column("is_resolved")]
    public bool IsResolved { get; set; } = false;

    [Column("view_count")]
    public int ViewCount { get; set; } = 0;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    [ForeignKey("LessonId")]
    public virtual Lesson Lesson { get; set; } = null!;

    [ForeignKey("CreatorId")]
    public virtual Account Creator { get; set; } = null!;

    public virtual ICollection<DiscussionComment> Comments { get; set; } = new List<DiscussionComment>();
}