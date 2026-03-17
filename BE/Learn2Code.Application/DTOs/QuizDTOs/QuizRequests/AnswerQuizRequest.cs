using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Learn2Code.Application.DTOs.QuizDTOs.QuizRequests;

public class AnswerQuizRequest
{
    [Required]
    [JsonPropertyName("option_id")]
    public Guid OptionId { get; set; }
}
