using Learn2Code.Application.Base;
using Learn2Code.Application.DTOs;

namespace Learn2Code.Application.Interfaces;

public interface IEnrollmentService
{
    /// <summary>
    /// Get all enrollments for current student
    /// </summary>
    Task<ServiceResult<List<EnrollmentDto>>> GetMyEnrollmentsAsync(Guid studentId);

    /// <summary>
    /// Enroll student into a course (check subscription)
    /// </summary>
    Task<ServiceResult<EnrollmentDto>> EnrollCourseAsync(Guid studentId, CreateEnrollmentRequest request);

    /// <summary>
    /// Get enrollment detail with progress for current student
    /// </summary>
    Task<ServiceResult<EnrollmentDetailDto>> GetMyEnrollmentDetailAsync(Guid studentId, Guid courseId);

    /// <summary>
    /// Get all enrollments (Admin only)
    /// </summary>
    Task<ServiceResult<EnrollmentListDto>> GetAllEnrollmentsAsync();
}
