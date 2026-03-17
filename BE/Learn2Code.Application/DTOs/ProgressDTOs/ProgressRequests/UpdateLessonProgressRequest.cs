using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Learn2Code.Application.DTOs.ProgressDTOs.ProgressRequests;

public class UpdateLessonProgressRequest
{
    [Required]
    [JsonPropertyName("status")]
    [RegularExpression("^(NotStarted|InProgress|Completed)$", ErrorMessage = "Status must be one of: NotStarted, InProgress, Completed")]
    public string Status { get; set; } = string.Empty;
}
