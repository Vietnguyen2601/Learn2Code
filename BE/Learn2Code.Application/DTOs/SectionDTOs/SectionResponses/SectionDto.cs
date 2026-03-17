using System.Text.Json.Serialization;

namespace Learn2Code.Application.DTOs.SectionDTOs.SectionResponses;

public class SectionDto
{
    [JsonPropertyName("section_id")]
    public Guid SectionId { get; set; }

    [JsonPropertyName("course_id")]
    public Guid CourseId { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("order_number")]
    public int OrderNumber { get; set; }

    [JsonPropertyName("is_active")]
    public bool IsActive { get; set; }

    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; set; }

    [JsonPropertyName("updated_at")]
    public DateTime UpdatedAt { get; set; }
}
