using Learn2Code.Application.Base;
using Learn2Code.Application.DTOs.DiscussionDTOs.DiscussionRequests;
using Learn2Code.Application.DTOs.DiscussionDTOs.DiscussionResponses;
using Learn2Code.Application.Interfaces;
using Learn2Code.Application.Mapper;
using Learn2Code.Infrastructure.Persistence.UnitOfWork;

namespace Learn2Code.Application.Services;

public class DiscussionService : IDiscussionService
{
    private readonly IUnitOfWork _unitOfWork;

    public DiscussionService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ServiceResult<List<DiscussionDto>>> GetDiscussionsByLessonIdAsync(Guid lessonId)
    {
        // Ki?m tra lesson có t?n t?i không
        var lesson = await _unitOfWork.LessonRepository.GetByIdAsync(lessonId);
        if (lesson == null)
            return ServiceResult<List<DiscussionDto>>.NotFound("Lesson not found");

        // L?y danh sách discussions
        var discussions = await _unitOfWork.DiscussionRepository.GetDiscussionsByLessonIdAsync(lessonId);
        var discussionDtos = discussions.Select(d => d.ToDto()).ToList();

        return ServiceResult<List<DiscussionDto>>.Ok(discussionDtos);
    }

    public async Task<ServiceResult<DiscussionDetailDto>> GetDiscussionByIdAsync(Guid discussionId)
    {
        var discussion = await _unitOfWork.DiscussionRepository.GetDiscussionWithDetailsAsync(discussionId);
        if (discussion == null)
            return ServiceResult<DiscussionDetailDto>.NotFound("Discussion not found");

        return ServiceResult<DiscussionDetailDto>.Ok(discussion.ToDetailDto());
    }

    public async Task<ServiceResult<DiscussionDto>> CreateDiscussionAsync(Guid lessonId, Guid userId, CreateDiscussionRequest request)
    {
        // Ki?m tra lesson có t?n t?i không
        var lesson = await _unitOfWork.LessonRepository.GetByIdAsync(lessonId);
        if (lesson == null)
            return ServiceResult<DiscussionDto>.NotFound("Lesson not found");

        // Ki?m tra user có t?n t?i không
        var user = await _unitOfWork.AccountRepository.GetByIdAsync(userId);
        if (user == null)
            return ServiceResult<DiscussionDto>.NotFound("User not found");

        // T?o discussion m?i
        var discussion = request.ToEntity(lessonId, userId);

        // L?u vào database
        var result = await _unitOfWork.DiscussionRepository.CreateAsync(discussion);

        if (result <= 0)
            return ServiceResult<DiscussionDto>.Error("CREATE_FAILED", "Failed to create discussion", 500);

        // Reload discussion v?i details
        var createdDiscussion = await _unitOfWork.DiscussionRepository.GetDiscussionWithDetailsAsync(discussion.DiscussionId);
        return ServiceResult<DiscussionDto>.Created(createdDiscussion!.ToDto());
    }

    public async Task<ServiceResult<DiscussionDto>> UpdateDiscussionAsync(Guid discussionId, Guid userId, UpdateDiscussionRequest request)
    {
        var discussion = await _unitOfWork.DiscussionRepository.GetByIdAsync(discussionId);
        if (discussion == null)
            return ServiceResult<DiscussionDto>.NotFound("Discussion not found");

        // Ki?m tra quy?n - ch? creator ho?c admin có th? update
        if (discussion.CreatorId != userId)
        {
            var user = await _unitOfWork.AccountRepository.GetByIdAsync(userId);
            var isAdmin = user?.AccountRoles.Any(ar => ar.Role?.RoleName == "Admin") ?? false;
            if (!isAdmin)
                return ServiceResult<DiscussionDto>.Error("PERMISSION_DENIED", "You don't have permission to update this discussion", 403);
        }

        // C?p nh?t discussion
        discussion.UpdateDiscussion(request);

        // L?u thay ??i
        var result = await _unitOfWork.DiscussionRepository.UpdateAsync(discussion);

        if (result <= 0)
            return ServiceResult<DiscussionDto>.Error("UPDATE_FAILED", "Failed to update discussion", 500);

        // Reload discussion v?i details
        var updatedDiscussion = await _unitOfWork.DiscussionRepository.GetDiscussionWithDetailsAsync(discussionId);
        return ServiceResult<DiscussionDto>.Ok(updatedDiscussion!.ToDto());
    }

    public async Task<ServiceResult> DeleteDiscussionAsync(Guid discussionId, Guid userId)
    {
        var discussion = await _unitOfWork.DiscussionRepository.GetByIdAsync(discussionId);
        if (discussion == null)
            return ServiceResult.NotFound("Discussion not found");

        // Ki?m tra quy?n - ch? creator ho?c admin có th? delete
        if (discussion.CreatorId != userId)
        {
            var user = await _unitOfWork.AccountRepository.GetByIdAsync(userId);
            var isAdmin = user?.AccountRoles.Any(ar => ar.Role?.RoleName == "Admin") ?? false;
            if (!isAdmin)
                return ServiceResult.Error("PERMISSION_DENIED", "You don't have permission to delete this discussion", 403);
        }

        // Xóa discussion
        var result = await _unitOfWork.DiscussionRepository.RemoveAsync(discussion);

        if (!result)
            return ServiceResult.Error("DELETE_FAILED", "Failed to delete discussion", 500);

        return ServiceResult.Ok("Discussion deleted successfully");
    }

    public async Task<ServiceResult> IncreaseViewCountAsync(Guid discussionId)
    {
        var discussion = await _unitOfWork.DiscussionRepository.GetByIdAsync(discussionId);
        if (discussion == null)
            return ServiceResult.NotFound("Discussion not found");

        discussion.ViewCount++;
        _unitOfWork.DiscussionRepository.Update(discussion);
        var result = await _unitOfWork.SaveChangesAsync();

        if (result <= 0)
            return ServiceResult.Error("UPDATE_FAILED", "Failed to update view count", 500);

        return ServiceResult.Ok();
    }
}
