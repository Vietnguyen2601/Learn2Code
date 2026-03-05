using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Learn2Code.Domain.Enums;

namespace Learn2Code.Application.DTOs;

public class ExerciseDto
{
    [JsonPropertyName("exercise_id")]
    public Guid ExerciseId { get; set; }

    [JsonPropertyName("lesson_id")]
    public Guid LessonId { get; set; }

    [JsonPropertyName("order_number")]
    public int OrderNumber { get; set; }

    [JsonPropertyName("exercise_type")]
    public string ExerciseType { get; set; } = string.Empty;

    [JsonPropertyName("narrative")]
    public string Narrative { get; set; } = string.Empty;

    [JsonPropertyName("language")]
    public string? Language { get; set; }

    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; set; }

    [JsonPropertyName("updated_at")]
    public DateTime UpdatedAt { get; set; }
}

public class ExerciseDetailDto : ExerciseDto
{
    [JsonPropertyName("starter_code")]
    public string? StarterCode { get; set; }

    [JsonPropertyName("solution_code")]
    public string? SolutionCode { get; set; }

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

public class ExerciseMediaDto
{
    [JsonPropertyName("media_id")]
    public Guid MediaId { get; set; }

    [JsonPropertyName("media_type")]
    public string MediaType { get; set; } = string.Empty;

    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;

    [JsonPropertyName("caption")]
    public string? Caption { get; set; }

    [JsonPropertyName("order_number")]
    public int OrderNumber { get; set; }
}

public class CreateExerciseRequest
{
    [Required]
    [JsonPropertyName("exercise_type")]
    [RegularExpression("^(Reading|FreeCode|GradedCode)$", ErrorMessage = "Exercise type must be one of: Reading, FreeCode, GradedCode")]
    public string ExerciseType { get; set; } = string.Empty;

    [Required]
    [JsonPropertyName("narrative")]
    public string Narrative { get; set; } = string.Empty;

    [JsonPropertyName("language")]
    public string? Language { get; set; }

    [JsonPropertyName("starter_code")]
    public string? StarterCode { get; set; }

    [JsonPropertyName("solution_code")]
    public string? SolutionCode { get; set; }

    [JsonPropertyName("instruction")]
    public string? Instruction { get; set; }

    [JsonPropertyName("hint")]
    public string? Hint { get; set; }
}

public class UpdateExerciseRequest
{
    [JsonPropertyName("exercise_type")]
    [RegularExpression("^(Reading|FreeCode|GradedCode)$", ErrorMessage = "Exercise type must be one of: Reading, FreeCode, GradedCode")]
    public string? ExerciseType { get; set; }

    [JsonPropertyName("narrative")]
    public string? Narrative { get; set; }

    [JsonPropertyName("language")]
    public string? Language { get; set; }

    [JsonPropertyName("starter_code")]
    public string? StarterCode { get; set; }

    [JsonPropertyName("solution_code")]
    public string? SolutionCode { get; set; }

    [JsonPropertyName("instruction")]
    public string? Instruction { get; set; }

    [JsonPropertyName("hint")]
    public string? Hint { get; set; }

    [JsonPropertyName("order_number")]
    public int? OrderNumber { get; set; }
}
