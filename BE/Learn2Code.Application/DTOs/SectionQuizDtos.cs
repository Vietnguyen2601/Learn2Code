using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Learn2Code.Application.DTOs;

// --- Read (GET /sections/:id/section-quiz) ---

public class SectionQuizDto
{
    [JsonPropertyName("section_id")]
    public Guid SectionId { get; set; }

    [JsonPropertyName("section_title")]
    public string SectionTitle { get; set; } = string.Empty;

    [JsonPropertyName("total_questions")]
    public int TotalQuestions { get; set; }

    [JsonPropertyName("quizzes")]
    public List<SectionQuizQuestionDto> Quizzes { get; set; } = new();
}

public class SectionQuizQuestionDto
{
    [JsonPropertyName("quiz_id")]
    public Guid QuizId { get; set; }

    [JsonPropertyName("question")]
    public string Question { get; set; } = string.Empty;

    [JsonPropertyName("order_number")]
    public int OrderNumber { get; set; }

    [JsonPropertyName("options")]
    public List<SectionQuizOptionDto> Options { get; set; } = new();
}

public class SectionQuizOptionDto
{
    [JsonPropertyName("option_id")]
    public Guid OptionId { get; set; }

    [JsonPropertyName("content")]
    public string Content { get; set; } = string.Empty;
}

// --- Attempt (POST /sections/:id/section-quiz/attempt) ---

public class SubmitSectionQuizRequest
{
    [Required]
    [JsonPropertyName("answers")]
    public List<SectionQuizAnswerInput> Answers { get; set; } = new();
}

public class SectionQuizAnswerInput
{
    [Required]
    [JsonPropertyName("quiz_id")]
    public Guid QuizId { get; set; }

    [Required]
    [JsonPropertyName("option_id")]
    public Guid OptionId { get; set; }
}

// --- Result ---

public class SectionQuizAttemptResultDto
{
    [JsonPropertyName("attempt_id")]
    public Guid AttemptId { get; set; }

    [JsonPropertyName("section_id")]
    public Guid SectionId { get; set; }

    [JsonPropertyName("score")]
    public decimal Score { get; set; }

    [JsonPropertyName("is_passed")]
    public bool IsPassed { get; set; }

    [JsonPropertyName("attempted_at")]
    public DateTime AttemptedAt { get; set; }

    [JsonPropertyName("answers")]
    public List<SectionQuizAnswerResultDto> Answers { get; set; } = new();
}

public class SectionQuizAnswerResultDto
{
    [JsonPropertyName("quiz_id")]
    public Guid QuizId { get; set; }

    [JsonPropertyName("option_id")]
    public Guid OptionId { get; set; }

    [JsonPropertyName("is_correct")]
    public bool IsCorrect { get; set; }

    [JsonPropertyName("explanation")]
    public string? Explanation { get; set; }
}
