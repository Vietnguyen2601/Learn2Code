using System.Text.Json.Serialization;

namespace Learn2Code.Application.DTOs.SectionQuizDTOs.SectionQuizResponses;

public class SectionQuizQuestionDto
{
    [JsonPropertyName("quiz_id")]
    public Guid QuizId { get; set; }

    [JsonPropertyName("question")]
    public string Question { get; set; } = string.Empty;

    [JsonPropertyName("order_number")]
    public int OrderNumber { get; set; }

    [JsonPropertyName("options")]
    public List<SectionQuizOptionDto> Options { get; set; } = new();
}
