using System.Text.Json.Serialization;

namespace Learn2Code.Application.DTOs.EnrollmentDTOs.EnrollmentResponses;

public class EnrollmentDetailDto : EnrollmentDto
{
    [JsonPropertyName("course_title")]
    public string CourseTitle { get; set; } = string.Empty;

    [JsonPropertyName("course_difficulty")]
    public string? CourseDifficulty { get; set; }

    [JsonPropertyName("student_name")]
    public string? StudentName { get; set; }

    [JsonPropertyName("student_email")]
    public string StudentEmail { get; set; } = string.Empty;
}
