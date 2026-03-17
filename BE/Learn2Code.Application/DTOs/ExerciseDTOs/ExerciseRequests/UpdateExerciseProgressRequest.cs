using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Learn2Code.Application.DTOs.ExerciseDTOs.ExerciseRequests;

public class UpdateExerciseProgressRequest
{
    [Required]
    [JsonPropertyName("is_completed")]
    public bool IsCompleted { get; set; }
}
