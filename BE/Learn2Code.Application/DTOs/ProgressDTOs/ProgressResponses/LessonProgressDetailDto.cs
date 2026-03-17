using System.Text.Json.Serialization;

namespace Learn2Code.Application.DTOs.ProgressDTOs.ProgressResponses;

public class LessonProgressDetailDto
{
    [JsonPropertyName("lesson_id")]
    public Guid LessonId { get; set; }

    [JsonPropertyName("lesson_title")]
    public string LessonTitle { get; set; } = string.Empty;

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("exercises")]
    public List<ExerciseProgressSummaryDto> Exercises { get; set; } = new();
}
