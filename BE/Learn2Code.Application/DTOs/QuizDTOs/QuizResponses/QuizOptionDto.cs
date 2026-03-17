using System.Text.Json.Serialization;

namespace Learn2Code.Application.DTOs.QuizDTOs.QuizResponses;

public class QuizOptionDto
{
    [JsonPropertyName("option_id")]
    public Guid OptionId { get; set; }

    [JsonPropertyName("quiz_id")]
    public Guid QuizId { get; set; }

    [JsonPropertyName("content")]
    public string Content { get; set; } = string.Empty;

    [JsonPropertyName("is_correct")]
    public bool IsCorrect { get; set; }

    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; set; }
}
