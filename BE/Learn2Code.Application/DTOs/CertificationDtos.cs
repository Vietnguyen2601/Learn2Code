using System.Text.Json.Serialization;

namespace Learn2Code.Application.DTOs;

/// <summary>
/// Response DTO for certification
/// </summary>
public class CertificationDto
{
    [JsonPropertyName("certification_id")]
    public Guid CertificationId { get; set; }

    [JsonPropertyName("student_id")]
    public Guid StudentId { get; set; }

    [JsonPropertyName("student_name")]
    public string? StudentName { get; set; }

    [JsonPropertyName("student_email")]
    public string? StudentEmail { get; set; }

    [JsonPropertyName("course_id")]
    public Guid CourseId { get; set; }

    [JsonPropertyName("course_title")]
    public string? CourseTitle { get; set; }

    [JsonPropertyName("certificate_code")]
    public string CertificateCode { get; set; } = string.Empty;

    [JsonPropertyName("certificate_url")]
    public string? CertificateUrl { get; set; }

    [JsonPropertyName("issued_at")]
    public DateTime IssuedAt { get; set; }
}

/// <summary>
/// Response for public certificate verification
/// </summary>
public class CertificateVerificationDto
{
    [JsonPropertyName("is_valid")]
    public bool IsValid { get; set; }

    [JsonPropertyName("certificate_code")]
    public string CertificateCode { get; set; } = string.Empty;

    [JsonPropertyName("student_name")]
    public string? StudentName { get; set; }

    [JsonPropertyName("course_title")]
    public string? CourseTitle { get; set; }

    [JsonPropertyName("issued_at")]
    public DateTime? IssuedAt { get; set; }

    [JsonPropertyName("certificate_url")]
    public string? CertificateUrl { get; set; }
}

/// <summary>
/// Response for checking certification eligibility
/// </summary>
public class CertificationEligibilityDto
{
    [JsonPropertyName("is_eligible")]
    public bool IsEligible { get; set; }

    [JsonPropertyName("already_certified")]
    public bool AlreadyCertified { get; set; }

    [JsonPropertyName("existing_certificate_code")]
    public string? ExistingCertificateCode { get; set; }

    [JsonPropertyName("progress")]
    public CertificationProgressDto Progress { get; set; } = new();

    [JsonPropertyName("requirements")]
    public CertificationRequirementsDto Requirements { get; set; } = new();

    [JsonPropertyName("missing")]
    public List<string> Missing { get; set; } = new();
}

/// <summary>
/// Current progress toward certification
/// </summary>
public class CertificationProgressDto
{
    [JsonPropertyName("lesson_completion_pct")]
    public decimal LessonCompletionPct { get; set; }

    [JsonPropertyName("exercise_pass_pct")]
    public decimal ExercisePassPct { get; set; }

    [JsonPropertyName("section_quiz_avg_score")]
    public decimal SectionQuizAvgScore { get; set; }

    [JsonPropertyName("sections_with_quiz_attempt")]
    public int SectionsWithQuizAttempt { get; set; }

    [JsonPropertyName("total_sections_with_quiz")]
    public int TotalSectionsWithQuiz { get; set; }
}

/// <summary>
/// Course completion requirements
/// </summary>
public class CertificationRequirementsDto
{
    [JsonPropertyName("min_lesson_completion_pct")]
    public decimal MinLessonCompletionPct { get; set; }

    [JsonPropertyName("min_exercise_pass_pct")]
    public decimal MinExercisePassPct { get; set; }

    [JsonPropertyName("min_section_quiz_score")]
    public decimal MinSectionQuizScore { get; set; }

    [JsonPropertyName("require_all_section_quiz")]
    public bool RequireAllSectionQuiz { get; set; }
}

/// <summary>
/// Response after issuing a certificate
/// </summary>
public class IssueCertificationResultDto
{
    [JsonPropertyName("certified")]
    public bool Certified { get; set; }

    [JsonPropertyName("certificate_code")]
    public string? CertificateCode { get; set; }

    [JsonPropertyName("certificate_url")]
    public string? CertificateUrl { get; set; }

    [JsonPropertyName("message")]
    public string? Message { get; set; }

    [JsonPropertyName("missing")]
    public List<string>? Missing { get; set; }
}

/// <summary>
/// Response DTO for certificate template (Admin)
/// </summary>
public class CertificateTemplateDto
{
    [JsonPropertyName("template_id")]
    public Guid TemplateId { get; set; }

    [JsonPropertyName("course_id")]
    public Guid CourseId { get; set; }

    [JsonPropertyName("course_title")]
    public string? CourseTitle { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("background_image_url")]
    public string? BackgroundImageUrl { get; set; }

    [JsonPropertyName("signature_name")]
    public string? SignatureName { get; set; }

    [JsonPropertyName("signature_image_url")]
    public string? SignatureImageUrl { get; set; }

    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; set; }

    [JsonPropertyName("updated_at")]
    public DateTime UpdatedAt { get; set; }
}
