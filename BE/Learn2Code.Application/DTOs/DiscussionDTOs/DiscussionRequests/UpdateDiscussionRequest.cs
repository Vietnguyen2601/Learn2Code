namespace Learn2Code.Application.DTOs.DiscussionDTOs.DiscussionRequests;

public class UpdateDiscussionRequest
{
    /// <summary>
    /// Tiêu ?? c?a discussion
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// N?i dung c?a discussion
    /// </summary>
    public string? Content { get; set; }

    /// <summary>
    /// Có pin discussion không
    /// </summary>
    public bool? IsPinned { get; set; }

    /// <summary>
    /// ?ã resolve discussion ch?a
    /// </summary>
    public bool? IsResolved { get; set; }
}
