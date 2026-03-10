using Learn2Code.Application.Base;
using Learn2Code.Application.DTOs;
using Learn2Code.Application.Interfaces;
using Learn2Code.Application.Mapper;
using Learn2Code.Domain.Enums;
using Learn2Code.Infrastructure.Persistence.UnitOfWork;

namespace Learn2Code.Application.Services;

public class EnrollmentService : IEnrollmentService
{
    private readonly IUnitOfWork _unitOfWork;

    public EnrollmentService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ServiceResult<List<EnrollmentDetailDto>>> GetMyEnrollmentsAsync(Guid studentId)
    {
        var enrollments = await _unitOfWork.EnrollmentRepository.GetEnrollmentsByStudentAsync(studentId);
        var dtos = enrollments.Select(e => e.ToDetailDto()).ToList();
        return ServiceResult<List<EnrollmentDetailDto>>.Ok(dtos);
    }

    public async Task<ServiceResult<EnrollmentDetailDto>> GetEnrollmentByIdAsync(Guid enrollmentId, Guid userId, bool isAdmin)
    {
        var enrollment = await _unitOfWork.EnrollmentRepository.GetEnrollmentWithDetailsAsync(enrollmentId);
        if (enrollment == null)
            return ServiceResult<EnrollmentDetailDto>.NotFound("Enrollment not found");

        if (!isAdmin && enrollment.StudentId != userId)
            return ServiceResult<EnrollmentDetailDto>.Error("ACCESS_DENIED", "You don't have permission to view this enrollment", 403);

        return ServiceResult<EnrollmentDetailDto>.Ok(enrollment.ToDetailDto());
    }

    public async Task<ServiceResult<EnrollmentDto>> CreateEnrollmentAsync(Guid studentId, CreateEnrollmentRequest request)
    {
        var course = await _unitOfWork.CourseRepository.GetByIdAsync(request.CourseId);
        if (course == null || !course.IsActive)
            return ServiceResult<EnrollmentDto>.NotFound("Course not found or inactive");

        var existing = await _unitOfWork.EnrollmentRepository
            .GetEnrollmentByStudentAndCourseAsync(studentId, request.CourseId);
        if (existing != null)
            return ServiceResult<EnrollmentDto>.Error("ALREADY_ENROLLED", "You are already enrolled in this course");

        var subscription = await _unitOfWork.SubscriptionRepository.GetCurrentActiveAsync(studentId);
        if (subscription == null || subscription.Status != SubscriptionStatus.Active)
            return ServiceResult<EnrollmentDto>.Error("NO_ACTIVE_SUBSCRIPTION", "An active subscription is required to enroll in a course", 403);

        var enrollment = request.ToEntity(studentId, subscription.SubscriptionId);
        _unitOfWork.EnrollmentRepository.PrepareCreate(enrollment);
        await _unitOfWork.SaveChangesAsync();

        return ServiceResult<EnrollmentDto>.Created(enrollment.ToDto(), "Enrolled successfully");
    }

    public async Task<ServiceResult<List<EnrollmentDetailDto>>> GetAllEnrollmentsAsync()
    {
        var enrollments = await _unitOfWork.EnrollmentRepository.GetAllWithDetailsAsync();
        var dtos = enrollments.Select(e => e.ToDetailDto()).ToList();
        return ServiceResult<List<EnrollmentDetailDto>>.Ok(dtos);
    }
}
