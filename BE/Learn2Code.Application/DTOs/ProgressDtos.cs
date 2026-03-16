using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Learn2Code.Application.DTOs;

// --- Lesson Progress ---

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

public class UpdateLessonProgressRequest
{
    [Required]
    [JsonPropertyName("status")]
    [RegularExpression("^(NotStarted|InProgress|Completed)$", ErrorMessage = "Status must be one of: NotStarted, InProgress, Completed")]
    public string Status { get; set; } = string.Empty;
}

// --- Course Progress ---

public class CourseProgressDto
{
    [JsonPropertyName("enrollment_status")]
    public string EnrollmentStatus { get; set; } = string.Empty;

    [JsonPropertyName("progress_pct")]
    public decimal ProgressPct { get; set; }

    [JsonPropertyName("sections")]
    public List<SectionProgressDto> Sections { get; set; } = new();
}

public class SectionProgressDto
{
    [JsonPropertyName("section_id")]
    public Guid SectionId { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("lessons_total")]
    public int LessonsTotal { get; set; }

    [JsonPropertyName("lessons_completed")]
    public int LessonsCompleted { get; set; }

    [JsonPropertyName("section_quiz_unlocked")]
    public bool SectionQuizUnlocked { get; set; }

    [JsonPropertyName("section_quiz_passed")]
    public bool SectionQuizPassed { get; set; }

    [JsonPropertyName("section_quiz_score")]
    public decimal? SectionQuizScore { get; set; }
}

// --- Lesson Progress Detail ---

public class LessonProgressDetailDto
{
    [JsonPropertyName("lesson_id")]
    public Guid LessonId { get; set; }

    [JsonPropertyName("lesson_title")]
    public string LessonTitle { get; set; } = string.Empty;

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("exercises")]
    public List<ExerciseProgressSummaryDto> Exercises { get; set; } = new();
}

public class ExerciseProgressSummaryDto
{
    [JsonPropertyName("exercise_id")]
    public Guid ExerciseId { get; set; }

    [JsonPropertyName("exercise_type")]
    public string ExerciseType { get; set; } = string.Empty;

    [JsonPropertyName("order_number")]
    public int OrderNumber { get; set; }

    [JsonPropertyName("is_completed")]
    public bool IsCompleted { get; set; }

    [JsonPropertyName("is_passed")]
    public bool IsPassed { get; set; }
}
