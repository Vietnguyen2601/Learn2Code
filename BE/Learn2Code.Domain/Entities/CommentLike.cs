using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Learn2Code.Domain.Entities;

[Table("comment_likes")]
public class CommentLike
{
    [Key]
    [Column("like_id")]
    public Guid LikeId { get; set; }

    [Column("comment_id")]
    public Guid CommentId { get; set; }

    [Column("user_id")]
    public Guid UserId { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    [ForeignKey("CommentId")]
    public virtual DiscussionComment Comment { get; set; } = null!;

    [ForeignKey("UserId")]
    public virtual Account User { get; set; } = null!;

    // Composite key
    public override int GetHashCode()
    {
        return HashCode.Combine(CommentId, UserId);
    }
}