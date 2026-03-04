using Learn2Code.Application.DTOs;
using Learn2Code.Domain.Entities;
using Learn2Code.Domain.Enums;

namespace Learn2Code.Application.Mapper;

public static class ProgressMapper
{
    public static SectionProgressSummaryDto ToSectionProgressSummaryDto(
        this Section section,
        List<LessonProgress> lessonProgresses,
        SectionQuizAttempt? bestAttempt)
    {
        var lessonsInSection = section.Lessons?.Count ?? 0;
        var completedLessons = lessonProgresses
            .Count(lp => lp.Status == LessonProgressStatus.Completed);

        // Section quiz is unlocked when all lessons are completed
        var sectionQuizUnlocked = lessonsInSection > 0 && completedLessons == lessonsInSection;

        return new SectionProgressSummaryDto
        {
            SectionId = section.SectionId,
            Title = section.Title,
            LessonsTotal = lessonsInSection,
            LessonsCompleted = completedLessons,
            SectionQuizUnlocked = sectionQuizUnlocked,
            SectionQuizPassed = bestAttempt?.IsPassed ?? false,
            SectionQuizScore = bestAttempt?.Score
        };
    }

    public static ExerciseProgressItemDto ToExerciseProgressItemDto(
        this Exercise exercise,
        ExerciseProgress? progress)
    {
        return new ExerciseProgressItemDto
        {
            ExerciseId = exercise.ExerciseId,
            Narrative = exercise.Narrative,
            IsCompleted = progress?.IsCompleted ?? false,
            IsPassed = progress?.IsPassed ?? false,
            TotalTestCases = exercise.TestCases?.Count,
            CompletedAt = progress?.CompletedAt
        };
    }

    public static QuizProgressDto ToQuizProgressDto(
        this Quiz quiz,
        bool isAnswered,
        bool? isCorrect)
    {
        return new QuizProgressDto
        {
            QuizId = quiz.QuizId,
            Question = quiz.Question,
            IsAnswered = isAnswered,
            IsCorrect = isCorrect
        };
    }

    public static StudentCourseProgressDto ToStudentCourseProgressDto(
        this Enrollment enrollment,
        decimal progressPct)
    {
        return new StudentCourseProgressDto
        {
            StudentId = enrollment.StudentId,
            StudentName = enrollment.Student?.Name ?? "Unknown",
            Email = enrollment.Student?.Email ?? "Unknown",
            EnrollmentStatus = enrollment.Status,
            ProgressPct = progressPct,
            EnrolledAt = enrollment.EnrolledAt,
            CompletedAt = enrollment.CompletedAt
        };
    }
}
