using Learn2Code.Application.Base;
using Learn2Code.Application.DTOs.FeedbackDTOs.FeedbackRequests;
using Learn2Code.Application.DTOs.FeedbackDTOs.FeedbackResponses;
using Learn2Code.Application.DTOs.LeaderboardDTOs;

namespace Learn2Code.Application.Interfaces;

public interface IFeedbackService
{
    Task<ServiceResult<List<FeedbackDto>>> GetFeedbacksByCourseAsync(Guid courseId);
    Task<ServiceResult<FeedbackDto>> CreateFeedbackAsync(Guid courseId, Guid studentId, CreateFeedbackRequest request);
    Task<ServiceResult<FeedbackDto>> UpdateMyFeedbackAsync(Guid courseId, Guid studentId, UpdateFeedbackRequest request);
    Task<ServiceResult> DeleteMyFeedbackAsync(Guid courseId, Guid studentId);
    Task<ServiceResult> AdminDeleteFeedbackAsync(Guid courseId, Guid feedbackId);
}

public interface ILeaderboardService
{
    Task<ServiceResult<List<LeaderboardEntryDto>>> GetLeaderboardAsync(Guid courseId, int top = 50);
}
