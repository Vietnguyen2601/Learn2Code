using System.Text.Json.Serialization;

namespace Learn2Code.Application.DTOs.ExerciseDTOs.ExerciseResponses;

public class ExerciseProgressDto
{
    [JsonPropertyName("exprogress_id")]
    public Guid ExProgressId { get; set; }

    [JsonPropertyName("student_id")]
    public Guid StudentId { get; set; }

    [JsonPropertyName("exercise_id")]
    public Guid ExerciseId { get; set; }

    [JsonPropertyName("is_completed")]
    public bool IsCompleted { get; set; }

    [JsonPropertyName("is_passed")]
    public bool IsPassed { get; set; }

    [JsonPropertyName("last_code")]
    public string? LastCode { get; set; }

    [JsonPropertyName("completed_at")]
    public DateTime? CompletedAt { get; set; }

    [JsonPropertyName("updated_at")]
    public DateTime UpdatedAt { get; set; }
}
