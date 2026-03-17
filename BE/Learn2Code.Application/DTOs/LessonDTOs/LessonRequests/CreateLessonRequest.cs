using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Learn2Code.Application.DTOs.LessonDTOs.LessonRequests;

public class CreateLessonRequest
{
    [Required]
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("is_free_preview")]
    public bool IsFreePreview { get; set; } = false;
}
