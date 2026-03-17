using System.Text.Json.Serialization;

namespace Learn2Code.Application.DTOs.ExerciseDTOs.ExerciseResponses;

public class ExerciseDto
{
    [JsonPropertyName("exercise_id")]
    public Guid ExerciseId { get; set; }

    [JsonPropertyName("lesson_id")]
    public Guid LessonId { get; set; }

    [JsonPropertyName("order_number")]
    public int OrderNumber { get; set; }

    [JsonPropertyName("exercise_type")]
    public string ExerciseType { get; set; } = string.Empty;

    [JsonPropertyName("narrative")]
    public string Narrative { get; set; } = string.Empty;

    [JsonPropertyName("language")]
    public string? Language { get; set; }

    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; set; }

    [JsonPropertyName("updated_at")]
    public DateTime UpdatedAt { get; set; }
}
