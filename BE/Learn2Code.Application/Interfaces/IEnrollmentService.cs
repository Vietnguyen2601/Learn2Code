using Learn2Code.Application.Base;
using Learn2Code.Application.DTOs;

namespace Learn2Code.Application.Interfaces;

public interface IEnrollmentService
{
    Task<ServiceResult<List<EnrollmentDetailDto>>> GetMyEnrollmentsAsync(Guid studentId);
    Task<ServiceResult<EnrollmentDetailDto>> GetEnrollmentByIdAsync(Guid enrollmentId, Guid userId, bool isAdmin);
    Task<ServiceResult<EnrollmentDto>> CreateEnrollmentAsync(Guid studentId, CreateEnrollmentRequest request);
    Task<ServiceResult<List<EnrollmentDetailDto>>> GetAllEnrollmentsAsync();
}
