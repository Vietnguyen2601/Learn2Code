using Learn2Code.Application.DTOs;
using Learn2Code.Domain.Entities;

namespace Learn2Code.Application.Mapper;

public static class QuizMapper
{
    public static QuizDto ToQuizDto(this Quiz quiz, bool includeCorrectAnswer = false)
    {
        return new QuizDto
        {
            QuizId = quiz.QuizId,
            LessonId = quiz.LessonId,
            OrderNumber = quiz.OrderNumber,
            Question = quiz.Question,
            Options = quiz.Options.Select(o => new QuizOptionDto
            {
                OptionId = o.OptionId,
                Content = o.Content,
                IsCorrect = includeCorrectAnswer ? o.IsCorrect : null
            }).ToList()
        };
    }

    public static SectionQuizAttemptDto ToSectionQuizAttemptDto(
        this SectionQuizAttempt attempt,
        int totalQuestions,
        int correctAnswers)
    {
        return new SectionQuizAttemptDto
        {
            AttemptId = attempt.AttemptId,
            SectionId = attempt.SectionId,
            SectionTitle = attempt.Section?.Title ?? string.Empty,
            Score = attempt.Score,
            IsPassed = attempt.IsPassed,
            AttemptedAt = attempt.AttemptedAt,
            TotalQuestions = totalQuestions,
            CorrectAnswers = correctAnswers
        };
    }
}
