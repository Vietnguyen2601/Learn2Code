using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Learn2Code.Domain.Entities;

[Table("discussion_comments")]
public class DiscussionComment
{
    [Key]
    [Column("comment_id")]
    public Guid CommentId { get; set; }

    [Column("discussion_id")]
    public Guid DiscussionId { get; set; }

    [Column("author_id")]
    public Guid AuthorId { get; set; }

    [Column("parent_comment_id")]
    public Guid? ParentCommentId { get; set; }

    [Required]
    [Column("content")]
    public string Content { get; set; } = string.Empty;

    [Column("like_count")]
    public int LikeCount { get; set; } = 0;

    [Column("is_answer")]
    public bool IsAnswer { get; set; } = false;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    [ForeignKey("DiscussionId")]
    public virtual Discussion Discussion { get; set; } = null!;

    [ForeignKey("AuthorId")]
    public virtual Account Author { get; set; } = null!;

    [ForeignKey("ParentCommentId")]
    public virtual DiscussionComment? ParentComment { get; set; }

    public virtual ICollection<DiscussionComment> RepliedComments { get; set; } = new List<DiscussionComment>();
    public virtual ICollection<CommentLike> Likes { get; set; } = new List<CommentLike>();
}