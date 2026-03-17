using System.Text.Json.Serialization;

namespace Learn2Code.Application.DTOs.ProgressDTOs.ProgressResponses;

public class ExerciseProgressSummaryDto
{
    [JsonPropertyName("exercise_id")]
    public Guid ExerciseId { get; set; }

    [JsonPropertyName("exercise_type")]
    public string ExerciseType { get; set; } = string.Empty;

    [JsonPropertyName("order_number")]
    public int OrderNumber { get; set; }

    [JsonPropertyName("is_completed")]
    public bool IsCompleted { get; set; }

    [JsonPropertyName("is_passed")]
    public bool IsPassed { get; set; }
}
