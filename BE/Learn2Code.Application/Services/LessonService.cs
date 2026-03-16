using Learn2Code.Application.Base;
using Learn2Code.Application.DTOs;
using Learn2Code.Application.Interfaces;
using Learn2Code.Application.Mapper;
using Learn2Code.Infrastructure.Persistence.UnitOfWork;

namespace Learn2Code.Application.Services;

public class LessonService : ILessonService
{
    private readonly IUnitOfWork _unitOfWork;

    public LessonService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ServiceResult<List<LessonDto>>> GetLessonsBySectionIdAsync(Guid sectionId)
    {
        // Ki?m tra section có t?n t?i không
        var section = await _unitOfWork.SectionRepository.GetByIdAsync(sectionId);
        if (section == null)
            return ServiceResult<List<LessonDto>>.NotFound("Section not found");

        var lessons = await _unitOfWork.LessonRepository.GetLessonsBySectionIdAsync(sectionId);
        var lessonDtos = lessons.Select(l => l.ToDto()).ToList();

        return ServiceResult<List<LessonDto>>.Ok(lessonDtos);
    }

    public async Task<ServiceResult<LessonDetailDto>> GetLessonByIdAsync(Guid lessonId, Guid? userId)
    {
        var lesson = await _unitOfWork.LessonRepository.GetLessonWithDetailsAsync(lessonId);
        if (lesson == null)
            return ServiceResult<LessonDetailDto>.NotFound("Lesson not found");

        // Ki?m tra quy?n truy c?p
        var canAccess = await _unitOfWork.LessonRepository.CanUserAccessLessonAsync(lessonId, userId);
        if (!canAccess)
            return ServiceResult<LessonDetailDto>.Error("ACCESS_DENIED", "You don't have permission to access this lesson", 403);

        return ServiceResult<LessonDetailDto>.Ok(lesson.ToDetailDto());
    }

    public async Task<ServiceResult<LessonDto>> CreateLessonAsync(Guid sectionId, CreateLessonRequest request)
    {
        // Ki?m tra section có t?n t?i không
        var section = await _unitOfWork.SectionRepository.GetByIdAsync(sectionId);
        if (section == null)
            return ServiceResult<LessonDto>.NotFound("Section not found");

        // L?y order number ti?p theo
        var maxOrder = await _unitOfWork.LessonRepository.GetMaxOrderNumberInSectionAsync(sectionId);
        var newOrderNumber = maxOrder + 1;

        var lesson = request.ToEntity(sectionId, newOrderNumber);
        _unitOfWork.LessonRepository.PrepareCreate(lesson);
        await _unitOfWork.SaveChangesAsync();

        return ServiceResult<LessonDto>.Created(lesson.ToDto(), "Lesson created successfully");
    }

    public async Task<ServiceResult<LessonDto>> UpdateLessonAsync(Guid lessonId, UpdateLessonRequest request)
    {
        var lesson = await _unitOfWork.LessonRepository.GetByIdAsync(lessonId);
        if (lesson == null)
            return ServiceResult<LessonDto>.NotFound("Lesson not found");

        lesson.UpdateLesson(request);
        _unitOfWork.LessonRepository.PrepareUpdate(lesson);
        await _unitOfWork.SaveChangesAsync();

        return ServiceResult<LessonDto>.Ok(lesson.ToDto(), "Lesson updated successfully");
    }

    public async Task<ServiceResult> DeleteLessonAsync(Guid lessonId)
    {
        var lesson = await _unitOfWork.LessonRepository.GetByIdAsync(lessonId);
        if (lesson == null)
            return ServiceResult.NotFound("Lesson not found");

        _unitOfWork.LessonRepository.PrepareRemove(lesson);
        await _unitOfWork.SaveChangesAsync();

        return ServiceResult.Ok("Lesson deleted successfully");
    }

    public async Task<ServiceResult> ReorderLessonsAsync(Guid sectionId, ReorderLessonsRequest request)
    {
        // Ki?m tra section có t?n t?i không
        var section = await _unitOfWork.SectionRepository.GetByIdAsync(sectionId);
        if (section == null)
            return ServiceResult.NotFound("Section not found");

        // Ki?m tra t?t c? lessons có thu?c section này không
        foreach (var lessonOrder in request.LessonOrders)
        {
            var exists = await _unitOfWork.LessonRepository.ExistsInSectionAsync(sectionId, lessonOrder.LessonId);
            if (!exists)
                return ServiceResult.Error("INVALID_LESSON", $"Lesson {lessonOrder.LessonId} does not belong to this section");
        }

        try
        {
            await _unitOfWork.BeginTransactionAsync();

            // B??C 1: Set t?t c? lessons sang order_number âm t?m th?i (tránh unique constraint conflict)
            int tempOrderOffset = -1000;
            foreach (var lessonOrder in request.LessonOrders)
            {
                var lesson = await _unitOfWork.LessonRepository.GetByIdAsync(lessonOrder.LessonId);
                if (lesson != null)
                {
                    lesson.OrderNumber = tempOrderOffset;
                    lesson.UpdatedAt = DateTime.UtcNow;
                    _unitOfWork.LessonRepository.PrepareUpdate(lesson);
                    tempOrderOffset--;
                }
            }
            await _unitOfWork.SaveChangesAsync();

            // B??C 2: C?p nh?t order_number th?t
            foreach (var lessonOrder in request.LessonOrders)
            {
                var lesson = await _unitOfWork.LessonRepository.GetByIdAsync(lessonOrder.LessonId);
                if (lesson != null)
                {
                    lesson.OrderNumber = lessonOrder.OrderNumber;
                    lesson.UpdatedAt = DateTime.UtcNow;
                    _unitOfWork.LessonRepository.PrepareUpdate(lesson);
                }
            }
            await _unitOfWork.SaveChangesAsync();

            await _unitOfWork.CommitTransactionAsync();

            return ServiceResult.Ok("Lessons reordered successfully");
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            return ServiceResult.Error("REORDER_FAILED", $"Failed to reorder lessons: {ex.Message}", 500);
        }
    }
}
