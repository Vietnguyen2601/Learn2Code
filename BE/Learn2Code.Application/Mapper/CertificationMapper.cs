using Learn2Code.Application.DTOs;
using Learn2Code.Domain.Entities;

namespace Learn2Code.Application.Mapper;

public static class CertificationMapper
{
    public static CertificationDto ToDto(this Certification certification)
    {
        return new CertificationDto
        {
            CertificationId = certification.CertificationId,
            StudentId = certification.StudentId,
            StudentName = certification.Student?.Name ?? certification.Student?.Username,
            StudentEmail = certification.Student?.Email,
            CourseId = certification.CourseId,
            CourseTitle = certification.Course?.Title,
            CertificateCode = certification.CertificateCode,
            CertificateUrl = certification.CertificateUrl,
            IssuedAt = certification.IssuedAt
        };
    }

    public static CertificateVerificationDto ToVerificationDto(this Certification? certification, string code)
    {
        if (certification == null)
        {
            return new CertificateVerificationDto
            {
                IsValid = false,
                CertificateCode = code
            };
        }

        return new CertificateVerificationDto
        {
            IsValid = true,
            CertificateCode = certification.CertificateCode,
            StudentName = certification.Student?.Name ?? certification.Student?.Username,
            CourseTitle = certification.Course?.Title,
            IssuedAt = certification.IssuedAt,
            CertificateUrl = certification.CertificateUrl
        };
    }

    public static CertificationRequirementsDto ToRequirementsDto(this CourseCompletionRule? rule)
    {
        if (rule == null)
        {
            // Default requirements if no rule is set
            return new CertificationRequirementsDto
            {
                MinLessonCompletionPct = 100,
                MinExercisePassPct = 0,
                MinSectionQuizScore = 0,
                RequireAllSectionQuiz = false
            };
        }

        return new CertificationRequirementsDto
        {
            MinLessonCompletionPct = rule.MinLessonCompletionPct,
            MinExercisePassPct = rule.MinExercisePassPct,
            MinSectionQuizScore = rule.MinSectionQuizScore,
            RequireAllSectionQuiz = rule.RequireAllSectionQuiz
        };
    }

    /// <summary>
    /// Generate a unique certificate code
    /// Format: L2C-{YEAR}{MONTH}-{RANDOM_6_CHARS}
    /// Example: L2C-202506-A1B2C3
    /// </summary>
    public static string GenerateCertificateCode()
    {
        var now = DateTime.UtcNow;
        var randomPart = Guid.NewGuid().ToString("N")[..6].ToUpperInvariant();
        return $"L2C-{now:yyyyMM}-{randomPart}";
    }

    public static CertificateTemplateDto ToTemplateDto(this CertificateTemplate template)
    {
        return new CertificateTemplateDto
        {
            TemplateId = template.TemplateId,
            CourseId = template.CourseId,
            CourseTitle = template.Course?.Title,
            Title = template.Title,
            Description = template.Description,
            BackgroundImageUrl = template.BackgroundImageUrl,
            SignatureName = template.SignatureName,
            SignatureImageUrl = template.SignatureImageUrl,
            CreatedAt = template.CreatedAt,
            UpdatedAt = template.UpdatedAt
        };
    }
}
