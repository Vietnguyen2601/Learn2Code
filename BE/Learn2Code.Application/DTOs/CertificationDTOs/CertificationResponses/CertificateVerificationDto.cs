using System.Text.Json.Serialization;

namespace Learn2Code.Application.DTOs.CertificationDTOs.CertificationResponses;

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
