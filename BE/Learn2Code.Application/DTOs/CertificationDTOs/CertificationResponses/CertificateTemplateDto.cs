using System.Text.Json.Serialization;

namespace Learn2Code.Application.DTOs.CertificationDTOs.CertificationResponses;

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
