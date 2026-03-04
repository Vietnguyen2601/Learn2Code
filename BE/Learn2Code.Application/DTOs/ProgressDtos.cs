using Learn2Code.Domain.Enums;
using System.Text.Json.Serialization;

namespace Learn2Code.Application.DTOs;

public class CourseProgressDto
{
    [JsonPropertyName("enrollment_status")]
    public EnrollmentStatus EnrollmentStatus { get; set; }
    
    [JsonPropertyName("progress_pct")]
    public decimal ProgressPct { get; set; }
    
    [JsonPropertyName("sections")]
    public List<SectionProgressSummaryDto> Sections { get; set; } = new();
}

public class SectionProgressSummaryDto
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

public class LessonProgressDetailDto
{
    public Guid LessonId { get; set; }
    public string Title { get; set; } = string.Empty;
    public LessonProgressStatus Status { get; set; }
    public List<ExerciseProgressItemDto> Exercises { get; set; } = new();
    public QuizProgressDto? Quiz { get; set; }
}

public class ExerciseProgressItemDto
{
    [JsonPropertyName("exercise_id")]
    public Guid ExerciseId { get; set; }
    
    [JsonPropertyName("narrative")]
    public string Narrative { get; set; } = string.Empty;
    
    [JsonPropertyName("is_completed")]
    public bool IsCompleted { get; set; }
    
    [JsonPropertyName("is_passed")]
    public bool IsPassed { get; set; }
    
    [JsonPropertyName("total_test_cases")]
    public int? TotalTestCases { get; set; }
    
    [JsonPropertyName("completed_at")]
    public DateTime? CompletedAt { get; set; }
}

public class QuizProgressDto
{
    public Guid QuizId { get; set; }
    public string Question { get; set; } = string.Empty;
    public bool IsAnswered { get; set; }
    public bool? IsCorrect { get; set; }
}

public class StudentCourseProgressDto
{
    public Guid StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public EnrollmentStatus EnrollmentStatus { get; set; }
    public decimal ProgressPct { get; set; }
    public DateTime EnrolledAt { get; set; }
    public DateTime? CompletedAt { get; set; }
}

public class AllStudentsProgressDto
{
    public Guid CourseId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public int TotalStudents { get; set; }
    public int CompletedStudents { get; set; }
    public decimal AverageProgress { get; set; }
    public List<StudentCourseProgressDto> Students { get; set; } = new();
}
