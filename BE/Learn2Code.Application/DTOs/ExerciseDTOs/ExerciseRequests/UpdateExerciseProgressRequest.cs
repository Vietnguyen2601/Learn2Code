using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Learn2Code.Application.DTOs.ExerciseDTOs.ExerciseRequests;

public class UpdateExerciseProgressRequest
{
    [Required]
    [JsonPropertyName("is_completed")]
    public bool IsCompleted { get; set; }

    [JsonPropertyName("is_passed")]
    public bool? IsPassed { get; set; }

    [JsonPropertyName("last_code")]
    public string? LastCode { get; set; }
}
