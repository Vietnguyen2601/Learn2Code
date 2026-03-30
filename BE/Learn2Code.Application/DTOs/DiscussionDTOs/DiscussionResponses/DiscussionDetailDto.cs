namespace Learn2Code.Application.DTOs.DiscussionDTOs.DiscussionResponses;

public class DiscussionDetailDto
{
    public Guid DiscussionId { get; set; }

    public Guid LessonId { get; set; }

    public string LessonTitle { get; set; } = string.Empty;

    public Guid CreatorId { get; set; }

    public string CreatorName { get; set; } = string.Empty;

    public string CreatorEmail { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;

    public bool IsPinned { get; set; }

    public bool IsResolved { get; set; }

    public int ViewCount { get; set; }

    public int CommentCount { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}
