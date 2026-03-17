using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Learn2Code.Application.DTOs.ExerciseDTOs.ExerciseRequests;

public class SubmitCodeRequest
{
    [Required]
    [JsonPropertyName("code")]
    public string Code { get; set; } = string.Empty;
}
