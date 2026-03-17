using System.Text.Json.Serialization;

namespace Learn2Code.Application.DTOs.LessonDTOs.LessonResponses;

public class LessonDetailDto : LessonDto
{
    [JsonPropertyName("section_title")]
    public string SectionTitle { get; set; } = string.Empty;

    [JsonPropertyName("course_title")]
    public string CourseTitle { get; set; } = string.Empty;

    [JsonPropertyName("exercise_count")]
    public int ExerciseCount { get; set; }

    [JsonPropertyName("quiz_count")]
    public int QuizCount { get; set; }
}
