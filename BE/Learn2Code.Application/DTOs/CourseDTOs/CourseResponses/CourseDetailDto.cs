using System.Text.Json.Serialization;
using Learn2Code.Application.DTOs.SectionDTOs.SectionResponses;

namespace Learn2Code.Application.DTOs.CourseDTOs.CourseResponses;

public class CourseDetailDto
{
    [JsonPropertyName("course_id")]
    public Guid CourseId { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("difficulty")]
    public string? Difficulty { get; set; }

    [JsonPropertyName("is_active")]
    public bool IsActive { get; set; }

    [JsonPropertyName("instructor_id")]
    public Guid InstructorId { get; set; }

    [JsonPropertyName("instructor_name")]
    public string? InstructorName { get; set; }

    [JsonPropertyName("category_id")]
    public Guid? CategoryId { get; set; }

    [JsonPropertyName("category_name")]
    public string? CategoryName { get; set; }

    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; set; }

    [JsonPropertyName("updated_at")]
    public DateTime UpdatedAt { get; set; }

    [JsonPropertyName("sections")]
    public List<SectionDto> Sections { get; set; } = new();
}
