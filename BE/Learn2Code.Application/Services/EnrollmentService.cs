using Learn2Code.Application.Base;
using Learn2Code.Application.DTOs;
using Learn2Code.Application.Interfaces;
using Learn2Code.Application.Mapper;
using Learn2Code.Domain.Enums;
using Learn2Code.Infrastructure.Persistence.UnitOfWork;
using Microsoft.EntityFrameworkCore;

namespace Learn2Code.Application.Services;

public class EnrollmentService : IEnrollmentService
{
    private readonly IUnitOfWork _unitOfWork;

    public EnrollmentService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ServiceResult<List<EnrollmentDto>>> GetMyEnrollmentsAsync(Guid studentId)
    {
        var enrollments = await _unitOfWork.EnrollmentRepository.GetByStudentIdAsync(studentId);
        
        var enrollmentDtos = enrollments.Select(e => e.ToEnrollmentDto()).ToList();
        
        return ServiceResult<List<EnrollmentDto>>.Ok(enrollmentDtos);
    }

    public async Task<ServiceResult<EnrollmentDto>> EnrollCourseAsync(Guid studentId, CreateEnrollmentRequest request)
    {
        // Check if already enrolled
        var existingEnrollment = await _unitOfWork.EnrollmentRepository
            .GetByStudentAndCourseAsync(studentId, request.CourseId);
        
        if (existingEnrollment != null)
        {
            return ServiceResult<EnrollmentDto>.Error(
                "ALREADY_ENROLLED", 
                "You are already enrolled in this course", 
                400);
        }

        // Check if course exists
        var course = await _unitOfWork.Repository<Domain.Entities.Course>()
            .GetByIdAsync(request.CourseId);
        
        if (course == null)
        {
            return ServiceResult<EnrollmentDto>.NotFound("Course not found");
        }

        if (!course.IsActive)
        {
            return ServiceResult<EnrollmentDto>.Error(
                "COURSE_INACTIVE", 
                "This course is not active", 
                400);
        }

        // Check if student has active subscription
        var activeSubscription = await _unitOfWork.Repository<Domain.Entities.UserSubscription>()
            .GetAsync(us => us.UserId == studentId && us.Status == SubscriptionStatus.Active);

        Guid? subscriptionId = null;
        if (activeSubscription != null)
        {
            subscriptionId = activeSubscription.SubscriptionId;
        }

        // Create enrollment
        var enrollment = request.ToEnrollment(studentId, subscriptionId);
        
        _unitOfWork.EnrollmentRepository.PrepareCreate(enrollment);
        await _unitOfWork.CommitTransactionAsync();

        // Reload with course info
        var createdEnrollment = await _unitOfWork.EnrollmentRepository
            .GetByStudentAndCourseAsync(studentId, request.CourseId);

        return ServiceResult<EnrollmentDto>.Created(
            createdEnrollment!.ToEnrollmentDto(), 
            "Successfully enrolled in course");
    }

    public async Task<ServiceResult<EnrollmentDetailDto>> GetMyEnrollmentDetailAsync(Guid studentId, Guid courseId)
    {
        var enrollment = await _unitOfWork.EnrollmentRepository
            .GetDetailByStudentAndCourseAsync(studentId, courseId);
        
        if (enrollment == null)
        {
            return ServiceResult<EnrollmentDetailDto>.NotFound("Enrollment not found");
        }

        // Get lesson progresses for this course
        var lessonProgresses = await _unitOfWork.LessonProgressRepository
            .GetByStudentAndCourseAsync(studentId, courseId);

        var sections = enrollment.Course.Sections.ToList();
        
        var enrollmentDetail = enrollment.ToEnrollmentDetailDto(sections, lessonProgresses);

        return ServiceResult<EnrollmentDetailDto>.Ok(enrollmentDetail);
    }

    public async Task<ServiceResult<EnrollmentListDto>> GetAllEnrollmentsAsync()
    {
        var enrollments = await _unitOfWork.EnrollmentRepository.GetAllAsync();
        
        var enrollmentDtos = enrollments.Select(e => e.ToEnrollmentDto()).ToList();
        
        var result = new EnrollmentListDto
        {
            Enrollments = enrollmentDtos,
            TotalCount = enrollmentDtos.Count
        };

        return ServiceResult<EnrollmentListDto>.Ok(result);
    }
}
