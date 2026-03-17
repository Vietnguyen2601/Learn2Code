using System.Text.Json.Serialization;

namespace Learn2Code.Application.DTOs.QuizDTOs.QuizResponses;

public class AnswerQuizResultDto
{
    [JsonPropertyName("quiz_id")]
    public Guid QuizId { get; set; }

    [JsonPropertyName("option_id")]
    public Guid OptionId { get; set; }

    [JsonPropertyName("is_correct")]
    public bool IsCorrect { get; set; }

    [JsonPropertyName("explanation")]
    public string? Explanation { get; set; }
}
