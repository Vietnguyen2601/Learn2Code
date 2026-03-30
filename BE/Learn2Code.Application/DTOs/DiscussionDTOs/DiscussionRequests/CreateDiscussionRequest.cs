namespace Learn2Code.Application.DTOs.DiscussionDTOs.DiscussionRequests;

public class CreateDiscussionRequest
{
    /// <summary>
    /// Tiêu ?? c?a discussion
    /// </summary>
    public required string Title { get; set; }

    /// <summary>
    /// N?i dung c?a discussion
    /// </summary>
    public required string Content { get; set; }
}
