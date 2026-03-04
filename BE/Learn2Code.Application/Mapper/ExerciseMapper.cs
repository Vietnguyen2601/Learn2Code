using Learn2Code.Application.DTOs;
using Learn2Code.Domain.Entities;

namespace Learn2Code.Application.Mapper;

public static class ExerciseMapper
{
    public static ExerciseDto ToExerciseDto(this Exercise exercise)
    {
        return new ExerciseDto
        {
            ExerciseId = exercise.ExerciseId,
            LessonId = exercise.LessonId,
            OrderNumber = exercise.OrderNumber,
            ExerciseType = exercise.ExerciseType.ToString(),
            Narrative = exercise.Narrative,
            Language = exercise.Language,
            Instruction = exercise.Instruction,
            CreatedAt = exercise.CreatedAt
        };
    }

    public static ExerciseProgressDto ToExerciseProgressDto(
        this ExerciseProgress progress,
        bool lessonCompleted,
        string? message = null)
    {
        return new ExerciseProgressDto
        {
            ExerciseId = progress.ExerciseId,
            IsCompleted = progress.IsCompleted,
            IsPassed = progress.IsPassed,
            CompletedAt = progress.CompletedAt,
            LessonCompleted = lessonCompleted,
            Message = message
        };
    }

    public static void UpdateFromRequest(this ExerciseProgress progress, UpdateProgressRequest request)
    {
        if (request.IsCompleted.HasValue)
        {
            progress.IsCompleted = request.IsCompleted.Value;
            if (request.IsCompleted.Value && progress.CompletedAt == null)
            {
                progress.CompletedAt = DateTime.UtcNow;
            }
        }

        if (request.IsPassed.HasValue)
        {
            progress.IsPassed = request.IsPassed.Value;
        }

        if (request.LastCode != null)
        {
            progress.LastCode = request.LastCode;
        }

        progress.UpdatedAt = DateTime.UtcNow;
    }
}
