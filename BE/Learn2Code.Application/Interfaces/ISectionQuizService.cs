using Learn2Code.Application.Base;
using Learn2Code.Application.DTOs.SectionQuizDTOs.SectionQuizRequests;
using Learn2Code.Application.DTOs.SectionQuizDTOs.SectionQuizResponses;

namespace Learn2Code.Application.Interfaces;

public interface ISectionQuizService
{
    Task<ServiceResult<SectionQuizDto>> GetSectionQuizAsync(Guid sectionId, Guid studentId);
    Task<ServiceResult<SectionQuizAttemptResultDto>> SubmitSectionQuizAttemptAsync(Guid sectionId, Guid studentId, SubmitSectionQuizRequest request);
    Task<ServiceResult<List<SectionQuizAttemptResultDto>>> GetStudentAttemptsAsync(Guid sectionId, Guid studentId);
}
