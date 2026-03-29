using Learn2Code.Application.DTOs.DiscussionDTOs.DiscussionRequests;
using Learn2Code.Application.DTOs.DiscussionDTOs.DiscussionResponses;
using Learn2Code.Domain.Entities;

namespace Learn2Code.Application.Mapper;

public static class DiscussionMapper
{
    public static DiscussionDto ToDto(this Discussion discussion)
    {
        return new DiscussionDto
        {
            DiscussionId = discussion.DiscussionId,
            LessonId = discussion.LessonId,
            CreatorId = discussion.CreatorId,
            CreatorName = discussion.Creator?.Name ?? discussion.Creator?.Username ?? string.Empty,
            Title = discussion.Title,
            Content = discussion.Content,
            IsPinned = discussion.IsPinned,
            IsResolved = discussion.IsResolved,
            ViewCount = discussion.ViewCount,
            CommentCount = discussion.Comments?.Count ?? 0,
            CreatedAt = discussion.CreatedAt,
            UpdatedAt = discussion.UpdatedAt
        };
    }

    public static DiscussionDetailDto ToDetailDto(this Discussion discussion)
    {
        return new DiscussionDetailDto
        {
            DiscussionId = discussion.DiscussionId,
            LessonId = discussion.LessonId,
            LessonTitle = discussion.Lesson?.Title ?? string.Empty,
            CreatorId = discussion.CreatorId,
            CreatorName = discussion.Creator?.Name ?? discussion.Creator?.Username ?? string.Empty,
            CreatorEmail = discussion.Creator?.Email ?? string.Empty,
            Title = discussion.Title,
            Content = discussion.Content,
            IsPinned = discussion.IsPinned,
            IsResolved = discussion.IsResolved,
            ViewCount = discussion.ViewCount,
            CommentCount = discussion.Comments?.Count ?? 0,
            CreatedAt = discussion.CreatedAt,
            UpdatedAt = discussion.UpdatedAt
        };
    }

    public static Discussion ToEntity(this CreateDiscussionRequest request, Guid lessonId, Guid creatorId)
    {
        return new Discussion
        {
            DiscussionId = Guid.NewGuid(),
            LessonId = lessonId,
            CreatorId = creatorId,
            Title = request.Title,
            Content = request.Content,
            IsPinned = false,
            IsResolved = false,
            ViewCount = 0,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public static void UpdateDiscussion(this Discussion discussion, UpdateDiscussionRequest request)
    {
        if (!string.IsNullOrWhiteSpace(request.Title))
            discussion.Title = request.Title;

        if (!string.IsNullOrWhiteSpace(request.Content))
            discussion.Content = request.Content;

        if (request.IsPinned.HasValue)
            discussion.IsPinned = request.IsPinned.Value;

        if (request.IsResolved.HasValue)
            discussion.IsResolved = request.IsResolved.Value;

        discussion.UpdatedAt = DateTime.UtcNow;
    }
}
