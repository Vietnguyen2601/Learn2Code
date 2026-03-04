using System.Text.Json.Serialization;

namespace Learn2Code.Application.DTOs;

public class LessonDto
{
    [JsonPropertyName("lesson_id")]
    public Guid LessonId { get; set; }

    [JsonPropertyName("section_id")]
    public Guid SectionId { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("order_number")]
    public int OrderNumber { get; set; }

    [JsonPropertyName("is_free_preview")]
    public bool IsFreePreview { get; set; }

    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; set; }
}

public class LessonDetailDto
{
    [JsonPropertyName("lesson_id")]
    public Guid LessonId { get; set; }

    [JsonPropertyName("section_id")]
    public Guid SectionId { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("order_number")]
    public int OrderNumber { get; set; }

    [JsonPropertyName("is_free_preview")]
    public bool IsFreePreview { get; set; }

    [JsonPropertyName("is_accessible")]
    public bool IsAccessible { get; set; }

    [JsonPropertyName("access_message")]
    public string? AccessMessage { get; set; }

    [JsonPropertyName("exercises")]
    public List<ExerciseInLessonDto> Exercises { get; set; } = new();

    [JsonPropertyName("progress_status")]
    public string ProgressStatus { get; set; } = "NotStarted";

    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; set; }
}

public class ExerciseInLessonDto
{
    [JsonPropertyName("exercise_id")]
    public Guid ExerciseId { get; set; }

    [JsonPropertyName("order_number")]
    public int OrderNumber { get; set; }

    [JsonPropertyName("exercise_type")]
    public string ExerciseType { get; set; } = string.Empty;

    [JsonPropertyName("narrative")]
    public string Narrative { get; set; } = string.Empty;

    [JsonPropertyName("language")]
    public string? Language { get; set; }

    [JsonPropertyName("starter_code")]
    public string? StarterCode { get; set; }

    [JsonPropertyName("instruction")]
    public string? Instruction { get; set; }

    [JsonPropertyName("hint")]
    public string? Hint { get; set; }

    [JsonPropertyName("is_completed")]
    public bool IsCompleted { get; set; }

    [JsonPropertyName("is_passed")]
    public bool IsPassed { get; set; }

    [JsonPropertyName("last_code")]
    public string? LastCode { get; set; }

    [JsonPropertyName("media")]
    public List<ExerciseMediaDto> Media { get; set; } = new();
}

public class ExerciseMediaDto
{
    [JsonPropertyName("media_id")]
    public Guid MediaId { get; set; }

    [JsonPropertyName("media_type")]
    public string MediaType { get; set; } = string.Empty;

    [JsonPropertyName("media_url")]
    public string MediaUrl { get; set; } = string.Empty;

    [JsonPropertyName("order_number")]
    public int OrderNumber { get; set; }
}
