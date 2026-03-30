using System.Text.Json.Serialization;

namespace Learn2Code.Application.DTOs.ProgressDTOs.ProgressResponses;

public class SectionProgressDto
{
    [JsonPropertyName("section_id")]
    public Guid SectionId { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("lessons_total")]
    public int LessonsTotal { get; set; }

    [JsonPropertyName("lessons_completed")]
    public int LessonsCompleted { get; set; }
}
