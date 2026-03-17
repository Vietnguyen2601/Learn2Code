using System.Text.Json.Serialization;

namespace Learn2Code.Application.DTOs.CertificationDTOs.CertificationResponses;

/// <summary>
/// Current progress toward certification
/// </summary>
public class CertificationProgressDto
{
    [JsonPropertyName("lesson_completion_pct")]
    public decimal LessonCompletionPct { get; set; }

    [JsonPropertyName("exercise_pass_pct")]
    public decimal ExercisePassPct { get; set; }

    [JsonPropertyName("section_quiz_avg_score")]
    public decimal SectionQuizAvgScore { get; set; }

    [JsonPropertyName("sections_with_quiz_attempt")]
    public int SectionsWithQuizAttempt { get; set; }

    [JsonPropertyName("total_sections_with_quiz")]
    public int TotalSectionsWithQuiz { get; set; }
}
