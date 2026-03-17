using System.Text.Json.Serialization;

namespace Learn2Code.Application.DTOs.ProgressDTOs.ProgressResponses;

public class LessonProgressDto
{
    [JsonPropertyName("progress_id")]
    public Guid ProgressId { get; set; }

    [JsonPropertyName("student_id")]
    public Guid StudentId { get; set; }

    [JsonPropertyName("lesson_id")]
    public Guid LessonId { get; set; }

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("last_accessed_at")]
    public DateTime? LastAccessedAt { get; set; }

    [JsonPropertyName("completed_at")]
    public DateTime? CompletedAt { get; set; }

    [JsonPropertyName("updated_at")]
    public DateTime UpdatedAt { get; set; }
}
