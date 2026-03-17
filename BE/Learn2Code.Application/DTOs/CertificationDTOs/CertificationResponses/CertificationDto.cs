using System.Text.Json.Serialization;

namespace Learn2Code.Application.DTOs.CertificationDTOs.CertificationResponses;

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
