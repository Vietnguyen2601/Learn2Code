using System.Text.Json.Serialization;

namespace Learn2Code.Application.DTOs.QuizDTOs.QuizResponses;

public class QuizDto
{
    [JsonPropertyName("quiz_id")]
    public Guid QuizId { get; set; }

    [JsonPropertyName("lesson_id")]
    public Guid LessonId { get; set; }

    [JsonPropertyName("order_number")]
    public int OrderNumber { get; set; }

    [JsonPropertyName("question")]
    public string Question { get; set; } = string.Empty;

    [JsonPropertyName("explanation")]
    public string? Explanation { get; set; }

    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; set; }

    [JsonPropertyName("updated_at")]
    public DateTime UpdatedAt { get; set; }

    [JsonPropertyName("options")]
    public List<QuizOptionDto> Options { get; set; } = new();
}
