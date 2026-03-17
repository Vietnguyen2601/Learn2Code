using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Learn2Code.Application.DTOs.QuizDTOs.QuizRequests;

public class CreateQuizOptionRequest
{
    [Required]
    [JsonPropertyName("content")]
    public string Content { get; set; } = string.Empty;

    [JsonPropertyName("is_correct")]
    public bool IsCorrect { get; set; } = false;
}
