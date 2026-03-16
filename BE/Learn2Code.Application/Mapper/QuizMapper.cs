using Learn2Code.Application.DTOs;
using Learn2Code.Domain.Entities;

namespace Learn2Code.Application.Mapper;

public static class QuizMapper
{
    public static QuizDto ToDto(this Quiz quiz)
    {
        return new QuizDto
        {
            QuizId = quiz.QuizId,
            LessonId = quiz.LessonId,
            OrderNumber = quiz.OrderNumber,
            Question = quiz.Question,
            Explanation = quiz.Explanation,
            CreatedAt = quiz.CreatedAt,
            UpdatedAt = quiz.UpdatedAt,
            Options = quiz.Options?.Select(o => o.ToOptionDto()).ToList() ?? new()
        };
    }

    public static QuizOptionDto ToOptionDto(this QuizOption option)
    {
        return new QuizOptionDto
        {
            OptionId = option.OptionId,
            QuizId = option.QuizId,
            Content = option.Content,
            IsCorrect = option.IsCorrect,
            CreatedAt = option.CreatedAt
        };
    }

    public static Quiz ToEntity(this CreateQuizRequest request, Guid lessonId, int orderNumber)
    {
        return new Quiz
        {
            QuizId = Guid.NewGuid(),
            LessonId = lessonId,
            OrderNumber = orderNumber,
            Question = request.Question,
            Explanation = request.Explanation,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public static QuizOption ToOptionEntity(this CreateQuizOptionRequest request, Guid quizId)
    {
        return new QuizOption
        {
            OptionId = Guid.NewGuid(),
            QuizId = quizId,
            Content = request.Content,
            IsCorrect = request.IsCorrect,
            CreatedAt = DateTime.UtcNow
        };
    }

    public static void UpdateQuiz(this Quiz quiz, UpdateQuizRequest request)
    {
        if (!string.IsNullOrWhiteSpace(request.Question))
            quiz.Question = request.Question;

        if (request.Explanation != null)
            quiz.Explanation = request.Explanation;

        if (request.OrderNumber.HasValue)
            quiz.OrderNumber = request.OrderNumber.Value;

        quiz.UpdatedAt = DateTime.UtcNow;
    }

    public static void UpdateOption(this QuizOption option, UpdateQuizOptionRequest request)
    {
        option.Content = request.Content;
        option.IsCorrect = request.IsCorrect;
    }

    public static void UpdateOption(this QuizOption option, UpdateSingleQuizOptionRequest request)
    {
        option.Content = request.Content;
        option.IsCorrect = request.IsCorrect;
    }
}
