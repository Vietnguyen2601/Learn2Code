using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

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

    [JsonPropertyName("instruction")]
    public string? Instruction { get; set; }

    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; set; }
}

public class RunCodeRequest
{
    [Required]
    [JsonPropertyName("code")]
    public string Code { get; set; } = string.Empty;

    [Required]
    [JsonPropertyName("language")]
    public string Language { get; set; } = string.Empty;

    [JsonPropertyName("input")]
    public string? Input { get; set; }
}

public class RunCodeResponse
{
    [JsonPropertyName("output")]
    public string? Output { get; set; }

    [JsonPropertyName("runtime_ms")]
    public int RuntimeMs { get; set; }

    [JsonPropertyName("error")]
    public string? Error { get; set; }

    [JsonPropertyName("is_success")]
    public bool IsSuccess { get; set; }
}

public class SubmitCodeRequest
{
    [Required]
    [JsonPropertyName("code")]
    public string Code { get; set; } = string.Empty;

    [Required]
    [JsonPropertyName("language")]
    public string Language { get; set; } = string.Empty;
}

public class SubmitCodeResponse
{
    [JsonPropertyName("is_passed")]
    public bool IsPassed { get; set; }

    [JsonPropertyName("passed_count")]
    public int PassedCount { get; set; }

    [JsonPropertyName("total_count")]
    public int TotalCount { get; set; }

    [JsonPropertyName("results")]
    public List<TestCaseResultDto> Results { get; set; } = new();

    [JsonPropertyName("message")]
    public string? Message { get; set; }
}

public class TestCaseResultDto
{
    [JsonPropertyName("testcase_id")]
    public Guid TestCaseId { get; set; }

    [JsonPropertyName("is_passed")]
    public bool IsPassed { get; set; }

    [JsonPropertyName("expected_output")]
    public string? ExpectedOutput { get; set; }

    [JsonPropertyName("actual_output")]
    public string? ActualOutput { get; set; }

    [JsonPropertyName("error")]
    public string? Error { get; set; }

    [JsonPropertyName("runtime_ms")]
    public int RuntimeMs { get; set; }

    [JsonPropertyName("is_hidden")]
    public bool IsHidden { get; set; }
}

public class UpdateProgressRequest
{
    [JsonPropertyName("is_completed")]
    public bool? IsCompleted { get; set; }

    [JsonPropertyName("is_passed")]
    public bool? IsPassed { get; set; }

    [JsonPropertyName("last_code")]
    public string? LastCode { get; set; }
}

public class ExerciseProgressDto
{
    [JsonPropertyName("exercise_id")]
    public Guid ExerciseId { get; set; }

    [JsonPropertyName("is_completed")]
    public bool IsCompleted { get; set; }

    [JsonPropertyName("is_passed")]
    public bool IsPassed { get; set; }

    [JsonPropertyName("completed_at")]
    public DateTime? CompletedAt { get; set; }

    [JsonPropertyName("lesson_completed")]
    public bool LessonCompleted { get; set; }

    [JsonPropertyName("message")]
    public string? Message { get; set; }
}
