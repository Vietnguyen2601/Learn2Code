using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Learn2Code.Application.DTOs.FeedbackDTOs.FeedbackRequests;

public class UpdateFeedbackRequest
{
    [Range(1, 5)]
    [JsonPropertyName("rating")]
    public int? Rating { get; set; }

    [JsonPropertyName("comment")]
    public string? Comment { get; set; }
}
