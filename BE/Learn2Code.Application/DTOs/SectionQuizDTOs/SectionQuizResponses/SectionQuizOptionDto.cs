using System.Text.Json.Serialization;

namespace Learn2Code.Application.DTOs.SectionQuizDTOs.SectionQuizResponses;

public class SectionQuizOptionDto
{
    [JsonPropertyName("option_id")]
    public Guid OptionId { get; set; }

    [JsonPropertyName("content")]
    public string Content { get; set; } = string.Empty;
}
