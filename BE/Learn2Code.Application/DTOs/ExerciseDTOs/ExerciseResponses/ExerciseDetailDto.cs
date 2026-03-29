using System.Text.Json.Serialization;

namespace Learn2Code.Application.DTOs.ExerciseDTOs.ExerciseResponses;

public class ExerciseDetailDto : ExerciseDto
{
    [JsonPropertyName("starter_code")]
    public string? StarterCode { get; set; }

    [JsonPropertyName("solution_code")]
    public string? SolutionCode { get; set; }

    /// <summary>
    /// Chỉ trả về cho Admin — student không được thấy validator code.
    /// </summary>
    [JsonPropertyName("solution_validator")]
    public string? SolutionValidator { get; set; }

    [JsonPropertyName("instruction")]
    public string? Instruction { get; set; }

    [JsonPropertyName("hint")]
    public string? Hint { get; set; }

    [JsonPropertyName("lesson_title")]
    public string LessonTitle { get; set; } = string.Empty;

    [JsonPropertyName("section_title")]
    public string SectionTitle { get; set; } = string.Empty;

    [JsonPropertyName("course_title")]
    public string CourseTitle { get; set; } = string.Empty;

    [JsonPropertyName("test_case_count")]
    public int TestCaseCount { get; set; }

    [JsonPropertyName("media_count")]
    public int MediaCount { get; set; }

    [JsonPropertyName("medias")]
    public List<ExerciseMediaDto>? Medias { get; set; }
}
