using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Learn2Code.Domain.Enums;

namespace Learn2Code.Application.DTOs.ExerciseMediaDTOs.ExerciseMediaRequests;

public class CreateExerciseMediaRequest
{
    [Required]
    [JsonPropertyName("media_type")]
    public MediaType MediaType { get; set; }

    [Required]
    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;

    [JsonPropertyName("caption")]
    public string? Caption { get; set; }

    [JsonPropertyName("order_number")]
    public int OrderNumber { get; set; } = 1;
}
