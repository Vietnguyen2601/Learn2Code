using Learn2Code.Application.Base;
using Learn2Code.Application.DTOs.FeedbackDTOs.FeedbackRequests;
using Learn2Code.Application.DTOs.FeedbackDTOs.FeedbackResponses;
using Learn2Code.Application.Interfaces;
using Learn2Code.Application.Mapper;
using Learn2Code.Domain.Entities;
using Learn2Code.Infrastructure.Persistence.UnitOfWork;
using Microsoft.EntityFrameworkCore;

namespace Learn2Code.Application.Services;

public class FeedbackService : IFeedbackService
{
    private readonly IUnitOfWork _unitOfWork;

    public FeedbackService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ServiceResult<List<FeedbackDto>>> GetFeedbacksByCourseAsync(Guid courseId)
    {
        var course = await _unitOfWork.CourseRepository.GetByIdAsync(courseId);
        if (course == null)
            return ServiceResult<List<FeedbackDto>>.NotFound("Course not found");

        var feedbacks = await _unitOfWork.Repository<Feedback>()
            .GetAllQueryable()
            .Where(f => f.CourseId == courseId)
            .Include(f => f.Student)
            .OrderByDescending(f => f.CreatedAt)
            .ToListAsync();

        var dtos = feedbacks.Select(f => f.ToDto()).ToList();
        return ServiceResult<List<FeedbackDto>>.Ok(dtos);
    }

    public async Task<ServiceResult<FeedbackDto>> CreateFeedbackAsync(Guid courseId, Guid studentId, CreateFeedbackRequest request)
    {
        var course = await _unitOfWork.CourseRepository.GetByIdAsync(courseId);
        if (course == null)
            return ServiceResult<FeedbackDto>.NotFound("Course not found");

        // Check student has enrolled in the course
        var isEnrolled = await _unitOfWork.EnrollmentRepository
            .GetAllQueryable()
            .AnyAsync(e => e.CourseId == courseId && e.StudentId == studentId);
        if (!isEnrolled)
            return ServiceResult<FeedbackDto>.Error("NOT_ENROLLED",
                "You must be enrolled in this course to give feedback");

        // Check no duplicate feedback
        var exists = await _unitOfWork.Repository<Feedback>()
            .AnyAsync(f => f.CourseId == courseId && f.StudentId == studentId);
        if (exists)
            return ServiceResult<FeedbackDto>.Error("DUPLICATE_FEEDBACK",
                "You have already submitted feedback for this course. Use PATCH to update.");

        var feedback = request.ToEntity(courseId, studentId);
        _unitOfWork.Repository<Feedback>().PrepareCreate(feedback);
        await _unitOfWork.SaveChangesAsync();

        // Reload with Student navigation
        var created = await _unitOfWork.Repository<Feedback>()
            .GetAllQueryable()
            .Include(f => f.Student)
            .FirstOrDefaultAsync(f => f.FeedbackId == feedback.FeedbackId);

        return ServiceResult<FeedbackDto>.Created(created!.ToDto(), "Feedback submitted successfully");
    }

    public async Task<ServiceResult<FeedbackDto>> UpdateMyFeedbackAsync(Guid courseId, Guid studentId, UpdateFeedbackRequest request)
    {
        var feedback = await _unitOfWork.Repository<Feedback>()
            .GetAllQueryable()
            .Include(f => f.Student)
            .FirstOrDefaultAsync(f => f.CourseId == courseId && f.StudentId == studentId);

        if (feedback == null)
            return ServiceResult<FeedbackDto>.NotFound("You have not submitted feedback for this course");

        feedback.ApplyUpdate(request);
        _unitOfWork.Repository<Feedback>().PrepareUpdate(feedback);
        await _unitOfWork.SaveChangesAsync();

        return ServiceResult<FeedbackDto>.Ok(feedback.ToDto(), "Feedback updated successfully");
    }

    public async Task<ServiceResult> DeleteMyFeedbackAsync(Guid courseId, Guid studentId)
    {
        var feedback = await _unitOfWork.Repository<Feedback>()
            .GetAsync(f => f.CourseId == courseId && f.StudentId == studentId);

        if (feedback == null)
            return ServiceResult.NotFound("You have not submitted feedback for this course");

        _unitOfWork.Repository<Feedback>().PrepareRemove(feedback);
        await _unitOfWork.SaveChangesAsync();

        return ServiceResult.Ok("Feedback deleted successfully");
    }

    public async Task<ServiceResult> AdminDeleteFeedbackAsync(Guid courseId, Guid feedbackId)
    {
        var feedback = await _unitOfWork.Repository<Feedback>().GetByIdAsync(feedbackId);
        if (feedback == null || feedback.CourseId != courseId)
            return ServiceResult.NotFound("Feedback not found or does not belong to this course");

        _unitOfWork.Repository<Feedback>().PrepareRemove(feedback);
        await _unitOfWork.SaveChangesAsync();

        return ServiceResult.Ok("Feedback deleted successfully");
    }
}
