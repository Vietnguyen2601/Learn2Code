using System.ComponentModel.DataAnnotations;
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

    [JsonPropertyName("updated_at")]
    public DateTime UpdatedAt { get; set; }
}

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

public class CreateLessonRequest
{
    [Required]
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("is_free_preview")]
    public bool IsFreePreview { get; set; } = false;
}

public class UpdateLessonRequest
{
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    [JsonPropertyName("is_free_preview")]
    public bool? IsFreePreview { get; set; }

    [JsonPropertyName("order_number")]
    public int? OrderNumber { get; set; }
}

public class ReorderLessonsRequest
{
    [Required]
    [JsonPropertyName("lesson_orders")]
    public List<LessonOrderDto> LessonOrders { get; set; } = new();
}

public class LessonOrderDto
{
    [Required]
    [JsonPropertyName("lesson_id")]
    public Guid LessonId { get; set; }

    [Required]
    [JsonPropertyName("order_number")]
    public int OrderNumber { get; set; }
}
