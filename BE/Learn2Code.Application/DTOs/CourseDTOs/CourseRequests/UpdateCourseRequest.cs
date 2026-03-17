using System.Text.Json.Serialization;

namespace Learn2Code.Application.DTOs.CourseDTOs.CourseRequests;

public class UpdateCourseRequest
{
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("difficulty")]
    public string? Difficulty { get; set; }

    [JsonPropertyName("category_id")]
    public Guid? CategoryId { get; set; }

    [JsonPropertyName("is_active")]
    public bool? IsActive { get; set; }
}
