using Learn2Code.Application.DTOs.FeedbackDTOs.FeedbackRequests;
using Learn2Code.Application.DTOs.FeedbackDTOs.FeedbackResponses;
using Learn2Code.Domain.Entities;

namespace Learn2Code.Application.Mapper;

public static class FeedbackMapper
{
    public static FeedbackDto ToDto(this Feedback feedback)
    {
        return new FeedbackDto
        {
            FeedbackId = feedback.FeedbackId,
            CourseId = feedback.CourseId,
            StudentId = feedback.StudentId,
            StudentName = feedback.Student?.Name ?? feedback.Student?.Username,
            Rating = feedback.Rating,
            Comment = feedback.Comment,
            CreatedAt = feedback.CreatedAt,
            UpdatedAt = feedback.UpdatedAt
        };
    }

    public static Feedback ToEntity(this CreateFeedbackRequest request, Guid courseId, Guid studentId)
    {
        return new Feedback
        {
            FeedbackId = Guid.NewGuid(),
            CourseId = courseId,
            StudentId = studentId,
            Rating = request.Rating,
            Comment = request.Comment,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public static void ApplyUpdate(this Feedback feedback, UpdateFeedbackRequest request)
    {
        if (request.Rating.HasValue)
            feedback.Rating = request.Rating.Value;
        if (request.Comment != null)
            feedback.Comment = request.Comment;
        feedback.UpdatedAt = DateTime.UtcNow;
    }
}
