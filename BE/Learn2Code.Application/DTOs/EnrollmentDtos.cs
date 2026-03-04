using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Learn2Code.Domain.Enums;

namespace Learn2Code.Application.DTOs;

public class EnrollmentDto
{
    [JsonPropertyName("enrollment_id")]
    public Guid EnrollmentId { get; set; }

    [JsonPropertyName("student_id")]
    public Guid StudentId { get; set; }

    [JsonPropertyName("course_id")]
    public Guid CourseId { get; set; }

    [JsonPropertyName("course_title")]
    public string CourseTitle { get; set; } = string.Empty;

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("progress_pct")]
    public decimal ProgressPct { get; set; }

    [JsonPropertyName("enrolled_at")]
    public DateTime EnrolledAt { get; set; }

    [JsonPropertyName("completed_at")]
    public DateTime? CompletedAt { get; set; }
}

public class EnrollmentDetailDto
{
    [JsonPropertyName("enrollment_id")]
    public Guid EnrollmentId { get; set; }

    [JsonPropertyName("course_id")]
    public Guid CourseId { get; set; }

    [JsonPropertyName("course_title")]
    public string CourseTitle { get; set; } = string.Empty;

    [JsonPropertyName("course_description")]
    public string? CourseDescription { get; set; }

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("progress_pct")]
    public decimal ProgressPct { get; set; }

    [JsonPropertyName("enrolled_at")]
    public DateTime EnrolledAt { get; set; }

    [JsonPropertyName("completed_at")]
    public DateTime? CompletedAt { get; set; }

    [JsonPropertyName("total_lessons")]
    public int TotalLessons { get; set; }

    [JsonPropertyName("completed_lessons")]
    public int CompletedLessons { get; set; }

    [JsonPropertyName("sections")]
    public List<SectionProgressDto> Sections { get; set; } = new();
}

public class SectionProgressDto
{
    [JsonPropertyName("section_id")]
    public Guid SectionId { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("order_number")]
    public int OrderNumber { get; set; }

    [JsonPropertyName("total_lessons")]
    public int TotalLessons { get; set; }

    [JsonPropertyName("completed_lessons")]
    public int CompletedLessons { get; set; }

    [JsonPropertyName("lessons")]
    public List<LessonProgressDto> Lessons { get; set; } = new();
}

public class LessonProgressDto
{
    [JsonPropertyName("lesson_id")]
    public Guid LessonId { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("order_number")]
    public int OrderNumber { get; set; }

    [JsonPropertyName("status")]
    public string Status { get; set; } = "NotStarted";

    [JsonPropertyName("is_free_preview")]
    public bool IsFreePreview { get; set; }

    [JsonPropertyName("last_accessed_at")]
    public DateTime? LastAccessedAt { get; set; }

    [JsonPropertyName("completed_at")]
    public DateTime? CompletedAt { get; set; }
}

public class CreateEnrollmentRequest
{
    [Required]
    [JsonPropertyName("course_id")]
    public Guid CourseId { get; set; }
}

public class EnrollmentListDto
{
    [JsonPropertyName("enrollments")]
    public List<EnrollmentDto> Enrollments { get; set; } = new();

    [JsonPropertyName("total_count")]
    public int TotalCount { get; set; }
}
