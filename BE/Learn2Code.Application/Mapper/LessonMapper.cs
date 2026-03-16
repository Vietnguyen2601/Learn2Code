using Learn2Code.Application.DTOs;
using Learn2Code.Domain.Entities;

namespace Learn2Code.Application.Mapper;

public static class LessonMapper
{
    public static LessonDto ToDto(this Lesson lesson)
    {
        return new LessonDto
        {
            LessonId = lesson.LessonId,
            SectionId = lesson.SectionId,
            Title = lesson.Title,
            OrderNumber = lesson.OrderNumber,
            IsFreePreview = lesson.IsFreePreview,
            CreatedAt = lesson.CreatedAt,
            UpdatedAt = lesson.UpdatedAt
        };
    }

    public static LessonDetailDto ToDetailDto(this Lesson lesson)
    {
        return new LessonDetailDto
        {
            LessonId = lesson.LessonId,
            SectionId = lesson.SectionId,
            Title = lesson.Title,
            OrderNumber = lesson.OrderNumber,
            IsFreePreview = lesson.IsFreePreview,
            CreatedAt = lesson.CreatedAt,
            UpdatedAt = lesson.UpdatedAt,
            SectionTitle = lesson.Section?.Title ?? string.Empty,
            CourseTitle = lesson.Section?.Course?.Title ?? string.Empty,
            ExerciseCount = lesson.Exercises?.Count ?? 0,
            QuizCount = lesson.Quizzes?.Count ?? 0
        };
    }

    public static Lesson ToEntity(this CreateLessonRequest request, Guid sectionId, int orderNumber)
    {
        return new Lesson
        {
            LessonId = Guid.NewGuid(),
            SectionId = sectionId,
            Title = request.Title,
            IsFreePreview = request.IsFreePreview,
            OrderNumber = orderNumber,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public static void UpdateLesson(this Lesson lesson, UpdateLessonRequest request)
    {
        if (!string.IsNullOrWhiteSpace(request.Title))
            lesson.Title = request.Title;

        if (request.IsFreePreview.HasValue)
            lesson.IsFreePreview = request.IsFreePreview.Value;

        if (request.OrderNumber.HasValue)
            lesson.OrderNumber = request.OrderNumber.Value;

        lesson.UpdatedAt = DateTime.UtcNow;
    }
}
