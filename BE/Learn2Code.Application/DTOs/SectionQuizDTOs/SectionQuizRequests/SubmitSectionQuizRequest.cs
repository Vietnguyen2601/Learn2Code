using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Learn2Code.Application.DTOs.SectionQuizDTOs.SectionQuizRequests;

public class SubmitSectionQuizRequest
{
    [Required]
    [JsonPropertyName("answers")]
    public List<SectionQuizAnswerInput> Answers { get; set; } = new();
}
