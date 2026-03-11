using Learn2Code.Application.Base;
using Learn2Code.Application.DTOs;

namespace Learn2Code.Application.Interfaces;

public interface ICertificationService
{
    /// <summary>
    /// Get all certifications for a student [Student only]
    /// </summary>
    Task<ServiceResult<List<CertificationDto>>> GetMyCertificationsAsync(Guid studentId);

    /// <summary>
    /// Verify/view certificate by code [Public]
    /// </summary>
    Task<ServiceResult<CertificateVerificationDto>> VerifyCertificateAsync(string certificateCode);

    /// <summary>
    /// Get all certifications [Admin only]
    /// </summary>
    Task<ServiceResult<List<CertificationDto>>> GetAllCertificationsAsync();

    /// <summary>
    /// Check if student is eligible for certification [Student only]
    /// </summary>
    Task<ServiceResult<CertificationEligibilityDto>> CheckCertificationEligibilityAsync(Guid studentId, Guid courseId);

    /// <summary>
    /// Issue certificate if eligible (called internally after quiz attempt or manually)
    /// </summary>
    Task<ServiceResult<IssueCertificationResultDto>> TryIssueCertificateAsync(Guid studentId, Guid courseId);

    /// <summary>
    /// Get all certificate templates [Admin only]
    /// </summary>
    Task<ServiceResult<List<CertificateTemplateDto>>> GetAllCertificateTemplatesAsync();

    /// <summary>
    /// Get certificate template by course ID [Admin only]
    /// </summary>
    Task<ServiceResult<CertificateTemplateDto>> GetCertificateTemplateByCourseIdAsync(Guid courseId);
}
