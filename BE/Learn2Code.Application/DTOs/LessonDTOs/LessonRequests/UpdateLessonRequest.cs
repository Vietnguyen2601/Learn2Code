using System.Text.Json.Serialization;

namespace Learn2Code.Application.DTOs.LessonDTOs.LessonRequests;

public class UpdateLessonRequest
{
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    [JsonPropertyName("is_free_preview")]
    public bool? IsFreePreview { get; set; }

    [JsonPropertyName("order_number")]
    public int? OrderNumber { get; set; }
}
