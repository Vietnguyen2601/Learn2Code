using Learn2Code.Application.DTOs;
using Learn2Code.Domain.Entities;
using Learn2Code.Domain.Enums;

namespace Learn2Code.Application.Mapper;

public static class SectionQuizMapper
{
    public static SectionQuizDto ToSectionQuizDto(this Section section, List<Quiz> quizzes)
    {
        return new SectionQuizDto
        {
            SectionId = section.SectionId,
            SectionTitle = section.Title,
            TotalQuestions = quizzes.Count,
            Quizzes = quizzes.OrderBy(q => q.OrderNumber).Select(q => new SectionQuizQuestionDto
            {
                QuizId = q.QuizId,
                Question = q.Question,
                OrderNumber = q.OrderNumber,
                Options = q.Options.Select(o => new SectionQuizOptionDto
                {
                    OptionId = o.OptionId,
                    Content = o.Content
                }).ToList()
            }).ToList()
        };
    }

    public static SectionQuizAttemptResultDto ToResultDto(this SectionQuizAttempt attempt)
    {
        return new SectionQuizAttemptResultDto
        {
            AttemptId = attempt.AttemptId,
            SectionId = attempt.SectionId,
            Score = attempt.Score,
            IsPassed = attempt.IsPassed,
            AttemptedAt = attempt.AttemptedAt,
            Answers = attempt.Answers.Select(a => new SectionQuizAnswerResultDto
            {
                QuizId = a.QuizId,
                OptionId = a.OptionId,
                IsCorrect = a.IsCorrect,
                Explanation = a.Quiz?.Explanation
            }).ToList()
        };
    }
}
