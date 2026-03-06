using Learn2Code.Application.Base;
using Learn2Code.Application.DTOs;

namespace Learn2Code.Application.Interfaces;

public interface IQuizService
{
    Task<ServiceResult<List<QuizDto>>> GetQuizzesByLessonIdAsync(Guid lessonId);
    Task<ServiceResult<QuizDto>> CreateQuizAsync(Guid lessonId, CreateQuizRequest request);
    Task<ServiceResult<QuizDto>> UpdateQuizAsync(Guid quizId, UpdateQuizRequest request);
    Task<ServiceResult> DeleteQuizAsync(Guid quizId);
    Task<ServiceResult<QuizOptionDto>> UpdateQuizOptionAsync(Guid quizId, Guid optionId, UpdateSingleQuizOptionRequest request);
    Task<ServiceResult> DeleteQuizOptionAsync(Guid quizId, Guid optionId);
}
