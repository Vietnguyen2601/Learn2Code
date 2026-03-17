using System.Text.Json.Serialization;

namespace Learn2Code.Application.DTOs.LessonDTOs.LessonResponses;

public class LessonDto
{
    [JsonPropertyName("lesson_id")]
    public Guid LessonId { get; set; }

    [JsonPropertyName("section_id")]
    public Guid SectionId { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("order_number")]
    public int OrderNumber { get; set; }

    [JsonPropertyName("is_free_preview")]
    public bool IsFreePreview { get; set; }

    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; set; }

    [JsonPropertyName("updated_at")]
    public DateTime UpdatedAt { get; set; }
}
