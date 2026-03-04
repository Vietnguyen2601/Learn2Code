using Learn2Code.Application.DTOs;
using Learn2Code.Domain.Entities;
using Learn2Code.Domain.Enums;

namespace Learn2Code.Application.Mapper;

public static class LessonMapper
{
    public static LessonDto ToLessonDto(this Lesson lesson)
    {
        return new LessonDto
        {
            LessonId = lesson.LessonId,
            SectionId = lesson.SectionId,
            Title = lesson.Title,
            OrderNumber = lesson.OrderNumber,
            IsFreePreview = lesson.IsFreePreview,
            CreatedAt = lesson.CreatedAt
        };
    }

    public static LessonDetailDto ToLessonDetailDto(
        this Lesson lesson,
        bool isAccessible,
        string? accessMessage,
        List<Exercise> exercises,
        List<ExerciseProgress> exerciseProgresses,
        LessonProgress? lessonProgress)
    {
        var exerciseInLessonDtos = new List<ExerciseInLessonDto>();

        foreach (var exercise in exercises.OrderBy(e => e.OrderNumber))
        {
            var progress = exerciseProgresses.FirstOrDefault(ep => ep.ExerciseId == exercise.ExerciseId);

            exerciseInLessonDtos.Add(new ExerciseInLessonDto
            {
                ExerciseId = exercise.ExerciseId,
                OrderNumber = exercise.OrderNumber,
                ExerciseType = exercise.ExerciseType.ToString(),
                Narrative = exercise.Narrative,
                Language = exercise.Language,
                StarterCode = exercise.StarterCode,
                Instruction = exercise.Instruction,
                Hint = exercise.Hint,
                IsCompleted = progress?.IsCompleted ?? false,
                IsPassed = progress?.IsPassed ?? false,
                LastCode = progress?.LastCode,
                Media = exercise.ExerciseMedias.OrderBy(m => m.OrderNumber)
                    .Select(m => new ExerciseMediaDto
                    {
                        MediaId = m.MediaId,
                        MediaType = m.MediaType.ToString(),
                        MediaUrl = m.Url,
                        OrderNumber = m.OrderNumber
                    }).ToList()
            });
        }

        return new LessonDetailDto
        {
            LessonId = lesson.LessonId,
            SectionId = lesson.SectionId,
            Title = lesson.Title,
            OrderNumber = lesson.OrderNumber,
            IsFreePreview = lesson.IsFreePreview,
            IsAccessible = isAccessible,
            AccessMessage = accessMessage,
            Exercises = exerciseInLessonDtos,
            ProgressStatus = lessonProgress?.Status.ToString() ?? "NotStarted",
            CreatedAt = lesson.CreatedAt
        };
    }
}
