using Learn2Code.Application.DTOs;
using Learn2Code.Domain.Entities;
using Learn2Code.Domain.Enums;

namespace Learn2Code.Application.Mapper;

public static class ExerciseMapper
{
    public static ExerciseDto ToDto(this Exercise exercise)
    {
        return new ExerciseDto
        {
            ExerciseId = exercise.ExerciseId,
            LessonId = exercise.LessonId,
            OrderNumber = exercise.OrderNumber,
            ExerciseType = exercise.ExerciseType.ToString(),
            Narrative = exercise.Narrative,
            Language = exercise.Language,
            CreatedAt = exercise.CreatedAt,
            UpdatedAt = exercise.UpdatedAt
        };
    }

    public static ExerciseDetailDto ToDetailDto(this Exercise exercise)
    {
        return new ExerciseDetailDto
        {
            ExerciseId = exercise.ExerciseId,
            LessonId = exercise.LessonId,
            OrderNumber = exercise.OrderNumber,
            ExerciseType = exercise.ExerciseType.ToString(),
            Narrative = exercise.Narrative,
            Language = exercise.Language,
            StarterCode = exercise.StarterCode,
            SolutionCode = exercise.SolutionCode,
            Instruction = exercise.Instruction,
            Hint = exercise.Hint,
            CreatedAt = exercise.CreatedAt,
            UpdatedAt = exercise.UpdatedAt,
            LessonTitle = exercise.Lesson?.Title ?? string.Empty,
            SectionTitle = exercise.Lesson?.Section?.Title ?? string.Empty,
            CourseTitle = exercise.Lesson?.Section?.Course?.Title ?? string.Empty,
            TestCaseCount = exercise.TestCases?.Count ?? 0,
            MediaCount = exercise.ExerciseMedias?.Count ?? 0,
            Medias = exercise.ExerciseMedias?.Select(m => m.ToMediaDto()).ToList()
        };
    }

    public static ExerciseMediaDto ToMediaDto(this ExerciseMedia media)
    {
        return new ExerciseMediaDto
        {
            MediaId = media.MediaId,
            MediaType = media.MediaType.ToString(),
            Url = media.Url,
            Caption = media.Caption,
            OrderNumber = media.OrderNumber
        };
    }

    public static Exercise ToEntity(this CreateExerciseRequest request, Guid lessonId, int orderNumber)
    {
        return new Exercise
        {
            ExerciseId = Guid.NewGuid(),
            LessonId = lessonId,
            OrderNumber = orderNumber,
            ExerciseType = Enum.Parse<ExerciseType>(request.ExerciseType, true),
            Narrative = request.Narrative,
            Language = request.Language,
            StarterCode = request.StarterCode,
            SolutionCode = request.SolutionCode,
            Instruction = request.Instruction,
            Hint = request.Hint,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public static void UpdateExercise(this Exercise exercise, UpdateExerciseRequest request)
    {
        if (!string.IsNullOrWhiteSpace(request.ExerciseType))
            exercise.ExerciseType = Enum.Parse<ExerciseType>(request.ExerciseType, true);

        if (!string.IsNullOrWhiteSpace(request.Narrative))
            exercise.Narrative = request.Narrative;

        if (request.Language != null)
            exercise.Language = request.Language;

        if (request.StarterCode != null)
            exercise.StarterCode = request.StarterCode;

        if (request.SolutionCode != null)
            exercise.SolutionCode = request.SolutionCode;

        if (request.Instruction != null)
            exercise.Instruction = request.Instruction;

        if (request.Hint != null)
            exercise.Hint = request.Hint;

        if (request.OrderNumber.HasValue)
            exercise.OrderNumber = request.OrderNumber.Value;

        exercise.UpdatedAt = DateTime.UtcNow;
    }
}
