using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Learn2Code.Application.DTOs.LessonDTOs.LessonRequests;

public class ReorderLessonsRequest
{
    [Required]
    [JsonPropertyName("lesson_orders")]
    public List<LessonOrderDto> LessonOrders { get; set; } = new();
}
