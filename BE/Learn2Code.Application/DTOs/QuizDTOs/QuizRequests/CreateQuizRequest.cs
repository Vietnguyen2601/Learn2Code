using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Learn2Code.Application.DTOs.QuizDTOs.QuizRequests;

public class CreateQuizRequest
{
    [Required]
    [JsonPropertyName("question")]
    public string Question { get; set; } = string.Empty;

    [JsonPropertyName("explanation")]
    public string? Explanation { get; set; }

    [Required]
    [MinLength(2, ErrorMessage = "Quiz must have at least 2 options")]
    [JsonPropertyName("options")]
    public List<CreateQuizOptionRequest> Options { get; set; } = new();
}
