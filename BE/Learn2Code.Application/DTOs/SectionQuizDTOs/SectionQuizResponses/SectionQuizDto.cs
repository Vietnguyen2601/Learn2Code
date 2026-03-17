using System.Text.Json.Serialization;

namespace Learn2Code.Application.DTOs.SectionQuizDTOs.SectionQuizResponses;

public class SectionQuizDto
{
    [JsonPropertyName("section_id")]
    public Guid SectionId { get; set; }

    [JsonPropertyName("section_title")]
    public string SectionTitle { get; set; } = string.Empty;

    [JsonPropertyName("total_questions")]
    public int TotalQuestions { get; set; }

    [JsonPropertyName("quizzes")]
    public List<SectionQuizQuestionDto> Quizzes { get; set; } = new();
}
