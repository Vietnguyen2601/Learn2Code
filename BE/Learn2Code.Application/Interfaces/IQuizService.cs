using Learn2Code.Application.Base;
using Learn2Code.Application.DTOs;

namespace Learn2Code.Application.Interfaces;

public interface IQuizService
{
    /// <summary>
    /// Answer a single quiz in lesson
    /// </summary>
    Task<ServiceResult<AnswerQuizResponse>> AnswerQuizAsync(Guid quizId, Guid studentId, AnswerQuizRequest request);

    /// <summary>
    /// Get section quiz with all questions (check if unlocked)
    /// </summary>
    Task<ServiceResult<SectionQuizDto>> GetSectionQuizAsync(Guid sectionId, Guid studentId);

    /// <summary>
    /// Submit section quiz attempt
    /// </summary>
    Task<ServiceResult<SubmitSectionQuizResponse>> SubmitSectionQuizAsync(Guid sectionId, Guid studentId, SubmitSectionQuizRequest request);

    /// <summary>
    /// Get student's section quiz attempt history
    /// </summary>
    Task<ServiceResult<SectionQuizAttemptListDto>> GetMyAttemptsAsync(Guid sectionId, Guid studentId);
}
