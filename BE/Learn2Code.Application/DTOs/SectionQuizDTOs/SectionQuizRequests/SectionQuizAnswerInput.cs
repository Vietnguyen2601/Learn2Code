using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Learn2Code.Application.DTOs.SectionQuizDTOs.SectionQuizRequests;

public class SectionQuizAnswerInput
{
    [Required]
    [JsonPropertyName("quiz_id")]
    public Guid QuizId { get; set; }

    [Required]
    [JsonPropertyName("option_id")]
    public Guid OptionId { get; set; }
}
