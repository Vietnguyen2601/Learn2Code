using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Learn2Code.Application.DTOs.QuizDTOs.QuizRequests;

public class UpdateQuizRequest
{
    [JsonPropertyName("question")]
    public string? Question { get; set; }

    [JsonPropertyName("explanation")]
    public string? Explanation { get; set; }

    [JsonPropertyName("order_number")]
    public int? OrderNumber { get; set; }

    [MinLength(2, ErrorMessage = "Quiz must have at least 2 options")]
    [JsonPropertyName("options")]
    public List<UpdateQuizOptionRequest>? Options { get; set; }
}
