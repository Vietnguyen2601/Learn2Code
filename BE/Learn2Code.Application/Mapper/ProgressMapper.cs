using Learn2Code.Application.DTOs;
using Learn2Code.Domain.Entities;

namespace Learn2Code.Application.Mapper;

public static class ProgressMapper
{
    public static LessonProgressDto ToDto(this LessonProgress progress)
    {
        return new LessonProgressDto
        {
            ProgressId = progress.ProgressId,
            StudentId = progress.StudentId,
            LessonId = progress.LessonId,
            Status = progress.Status.ToString(),
            LastAccessedAt = progress.LastAccessedAt,
            CompletedAt = progress.CompletedAt,
            UpdatedAt = progress.UpdatedAt
        };
    }
}
