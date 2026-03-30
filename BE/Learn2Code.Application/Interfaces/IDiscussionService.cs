using Learn2Code.Application.Base;
using Learn2Code.Application.DTOs.DiscussionDTOs.DiscussionRequests;
using Learn2Code.Application.DTOs.DiscussionDTOs.DiscussionResponses;

namespace Learn2Code.Application.Interfaces;

public interface IDiscussionService
{
    /// <summary>
    /// L?y t?t c? discussion c?a m?t lesson
    /// </summary>
    Task<ServiceResult<List<DiscussionDto>>> GetDiscussionsByLessonIdAsync(Guid lessonId);

    /// <summary>
    /// L?y chi ti?t m?t discussion
    /// </summary>
    Task<ServiceResult<DiscussionDetailDto>> GetDiscussionByIdAsync(Guid discussionId);

    /// <summary>
    /// T?o discussion m?i
    /// </summary>
    Task<ServiceResult<DiscussionDto>> CreateDiscussionAsync(Guid lessonId, Guid userId, CreateDiscussionRequest request);

    /// <summary>
    /// C?p nh?t discussion
    /// </summary>
    Task<ServiceResult<DiscussionDto>> UpdateDiscussionAsync(Guid discussionId, Guid userId, UpdateDiscussionRequest request);

    /// <summary>
    /// Xóa discussion
    /// </summary>
    Task<ServiceResult> DeleteDiscussionAsync(Guid discussionId, Guid userId);

    /// <summary>
    /// T?ng view count
    /// </summary>
    Task<ServiceResult> IncreaseViewCountAsync(Guid discussionId);
}
