using Learn2Code.Application.Base;
using Learn2Code.Application.DTOs.CertificationDTOs.CertificationResponses;
using Learn2Code.Application.Interfaces;
using Learn2Code.Application.Mapper;
using Learn2Code.Domain.Entities;
using Learn2Code.Domain.Enums;
using Learn2Code.Infrastructure.Persistence.UnitOfWork;
using Microsoft.EntityFrameworkCore;

namespace Learn2Code.Application.Services;

public class CertificationService : ICertificationService
{
    private readonly IUnitOfWork _unitOfWork;

    public CertificationService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ServiceResult<List<CertificationDto>>> GetMyCertificationsAsync(Guid studentId)
    {
        var certifications = await _unitOfWork.CertificationRepository.GetByStudentIdAsync(studentId);
        var dtos = certifications.Select(c => c.ToDto()).ToList();
        return ServiceResult<List<CertificationDto>>.Ok(dtos);
    }

    public async Task<ServiceResult<CertificateVerificationDto>> VerifyCertificateAsync(string certificateCode)
    {
        if (string.IsNullOrWhiteSpace(certificateCode))
        {
            return ServiceResult<CertificateVerificationDto>.BadRequest("Certificate code is required");
        }

        var certification = await _unitOfWork.CertificationRepository.GetByCertificateCodeAsync(certificateCode);
        var dto = certification.ToVerificationDto(certificateCode);
        
        return ServiceResult<CertificateVerificationDto>.Ok(dto);
    }

    public async Task<ServiceResult<List<CertificationDto>>> GetAllCertificationsAsync()
    {
        var certifications = await _unitOfWork.CertificationRepository.GetAllWithDetailsAsync();
        var dtos = certifications.Select(c => c.ToDto()).ToList();
        return ServiceResult<List<CertificationDto>>.Ok(dtos);
    }

    public async Task<ServiceResult<CertificationEligibilityDto>> CheckCertificationEligibilityAsync(Guid studentId, Guid courseId)
    {
        // Check if course exists
        var course = await _unitOfWork.CourseRepository.GetByIdAsync(courseId);
        if (course == null)
        {
            return ServiceResult<CertificationEligibilityDto>.NotFound("Course not found");
        }

        // Check if student is enrolled
        var enrollment = await _unitOfWork.EnrollmentRepository.GetEnrollmentByStudentAndCourseAsync(studentId, courseId);
        if (enrollment == null)
        {
            return ServiceResult<CertificationEligibilityDto>.Error("NOT_ENROLLED", "You are not enrolled in this course");
        }

        // Check if already certified
        var existingCertification = await _unitOfWork.CertificationRepository.GetByStudentAndCourseAsync(studentId, courseId);
        if (existingCertification != null)
        {
            return ServiceResult<CertificationEligibilityDto>.Ok(new CertificationEligibilityDto
            {
                IsEligible = true,
                AlreadyCertified = true,
                ExistingCertificateCode = existingCertification.CertificateCode,
                Progress = new CertificationProgressDto(),
                Requirements = new CertificationRequirementsDto()
            });
        }

        // Get completion rules
        var rule = await _unitOfWork.Repository<CourseCompletionRule>()
            .GetAsync(r => r.CourseId == courseId);

        // Calculate progress and check eligibility
        var (progress, missing) = await CalculateProgressAsync(studentId, courseId, rule);
        var requirements = rule.ToRequirementsDto();

        var isEligible = missing.Count == 0;

        return ServiceResult<CertificationEligibilityDto>.Ok(new CertificationEligibilityDto
        {
            IsEligible = isEligible,
            AlreadyCertified = false,
            Progress = progress,
            Requirements = requirements,
            Missing = missing
        });
    }

    public async Task<ServiceResult<IssueCertificationResultDto>> TryIssueCertificateAsync(Guid studentId, Guid courseId)
    {
        // Check eligibility first
        var eligibilityResult = await CheckCertificationEligibilityAsync(studentId, courseId);
        if (!eligibilityResult.Success)
        {
            return ServiceResult<IssueCertificationResultDto>.Error(
                eligibilityResult.ErrorCode ?? "ERROR",
                eligibilityResult.Message ?? "Failed to check eligibility",
                eligibilityResult.Status);
        }

        var eligibility = eligibilityResult.Data!;

        // Already certified
        if (eligibility.AlreadyCertified)
        {
            return ServiceResult<IssueCertificationResultDto>.Ok(new IssueCertificationResultDto
            {
                Certified = true,
                CertificateCode = eligibility.ExistingCertificateCode,
                Message = "You already have a certificate for this course"
            });
        }

        // Not eligible
        if (!eligibility.IsEligible)
        {
            return ServiceResult<IssueCertificationResultDto>.Ok(new IssueCertificationResultDto
            {
                Certified = false,
                Missing = eligibility.Missing,
                Message = "You have not met all requirements for certification"
            });
        }

        // Issue certificate
        var certification = new Certification
        {
            CertificationId = Guid.NewGuid(),
            StudentId = studentId,
            CourseId = courseId,
            CertificateCode = CertificationMapper.GenerateCertificateCode(),
            CertificateUrl = null, // Can be set later when PDF is generated
            IssuedAt = DateTime.UtcNow
        };

        _unitOfWork.CertificationRepository.PrepareCreate(certification);

        // Update enrollment status to Completed
        var enrollment = await _unitOfWork.EnrollmentRepository.GetEnrollmentByStudentAndCourseAsync(studentId, courseId);
        if (enrollment != null)
        {
            enrollment.Status = EnrollmentStatus.Completed;
            enrollment.ProgressPct = 100;
            enrollment.CompletedAt = DateTime.UtcNow;
            _unitOfWork.EnrollmentRepository.PrepareUpdate(enrollment);
        }

        await _unitOfWork.SaveChangesAsync();

        return ServiceResult<IssueCertificationResultDto>.Created(new IssueCertificationResultDto
        {
            Certified = true,
            CertificateCode = certification.CertificateCode,
            CertificateUrl = certification.CertificateUrl,
            Message = "Congratulations! Certificate issued successfully"
        }, "Certificate issued successfully");
    }

    /// <summary>
    /// Calculate student's progress toward certification
    /// </summary>
    private async Task<(CertificationProgressDto progress, List<string> missing)> CalculateProgressAsync(
        Guid studentId, 
        Guid courseId, 
        CourseCompletionRule? rule)
    {
        var missing = new List<string>();
        var progress = new CertificationProgressDto();

        // Default requirements if no rule exists
        var minWeightScore = rule?.MinWeightScore ?? 0;

        // For now, no quiz requirements - return empty requirements
        return (progress, missing);
    }

    public async Task<ServiceResult<List<CertificateTemplateDto>>> GetAllCertificateTemplatesAsync()
    {
        var templates = await _unitOfWork.Repository<CertificateTemplate>()
            .GetAllQueryable()
            .Include(t => t.Course)
            .OrderBy(t => t.Course.Title)
            .ToListAsync();

        var dtos = templates.Select(t => t.ToTemplateDto()).ToList();
        return ServiceResult<List<CertificateTemplateDto>>.Ok(dtos);
    }

    public async Task<ServiceResult<CertificateTemplateDto>> GetCertificateTemplateByCourseIdAsync(Guid courseId)
    {
        var template = await _unitOfWork.Repository<CertificateTemplate>()
            .GetAllQueryable()
            .Include(t => t.Course)
            .FirstOrDefaultAsync(t => t.CourseId == courseId);

        if (template == null)
            return ServiceResult<CertificateTemplateDto>.NotFound("Certificate template not found for this course");

        return ServiceResult<CertificateTemplateDto>.Ok(template.ToTemplateDto());
    }
}
