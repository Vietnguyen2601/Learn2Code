using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Learn2Code.Application.DTOs.LessonDTOs.LessonRequests;

public class LessonOrderDto
{
    [Required]
    [JsonPropertyName("lesson_id")]
    public Guid LessonId { get; set; }

    [Required]
    [JsonPropertyName("order_number")]
    public int OrderNumber { get; set; }
}
