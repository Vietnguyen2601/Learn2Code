using System.Text.Json.Serialization;

namespace Learn2Code.Application.DTOs.FeedbackDTOs.FeedbackResponses;

public class FeedbackDto
{
    [JsonPropertyName("feedback_id")]
    public Guid FeedbackId { get; set; }

    [JsonPropertyName("course_id")]
    public Guid CourseId { get; set; }

    [JsonPropertyName("student_id")]
    public Guid StudentId { get; set; }

    [JsonPropertyName("student_name")]
    public string? StudentName { get; set; }

    [JsonPropertyName("rating")]
    public int Rating { get; set; }

    [JsonPropertyName("comment")]
    public string? Comment { get; set; }

    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; set; }

    [JsonPropertyName("updated_at")]
    public DateTime UpdatedAt { get; set; }
}
