using System.Text.Json.Serialization;

namespace Learn2Code.Application.DTOs.CertificationDTOs.CertificationResponses;

/// <summary>
/// Course completion requirements
/// </summary>
public class CertificationRequirementsDto
{
    [JsonPropertyName("min_lesson_completion_pct")]
    public decimal MinLessonCompletionPct { get; set; }

    [JsonPropertyName("min_exercise_pass_pct")]
    public decimal MinExercisePassPct { get; set; }

    [JsonPropertyName("min_section_quiz_score")]
    public decimal MinSectionQuizScore { get; set; }

    [JsonPropertyName("require_all_section_quiz")]
    public bool RequireAllSectionQuiz { get; set; }
}
