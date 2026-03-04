using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Learn2Code.Application.DTOs;

// ========== Quiz trong Lesson ==========

public class QuizDto
{
    [JsonPropertyName("quiz_id")]
    public Guid QuizId { get; set; }

    [JsonPropertyName("lesson_id")]
    public Guid LessonId { get; set; }

    [JsonPropertyName("order_number")]
    public int OrderNumber { get; set; }

    [JsonPropertyName("question")]
    public string Question { get; set; } = string.Empty;

    [JsonPropertyName("options")]
    public List<QuizOptionDto> Options { get; set; } = new();
}

public class QuizOptionDto
{
    [JsonPropertyName("option_id")]
    public Guid OptionId { get; set; }

    [JsonPropertyName("content")]
    public string Content { get; set; } = string.Empty;

    [JsonPropertyName("is_correct")]
    public bool? IsCorrect { get; set; } // null when showing to student, true/false after answer
}

public class AnswerQuizRequest
{
    [Required]
    [JsonPropertyName("option_id")]
    public Guid OptionId { get; set; }
}

public class AnswerQuizResponse
{
    [JsonPropertyName("quiz_id")]
    public Guid QuizId { get; set; }

    [JsonPropertyName("is_correct")]
    public bool IsCorrect { get; set; }

    [JsonPropertyName("explanation")]
    public string? Explanation { get; set; }

    [JsonPropertyName("correct_option_id")]
    public Guid CorrectOptionId { get; set; }
}

// ========== Section Quiz ==========

public class SectionQuizDto
{
    [JsonPropertyName("section_id")]
    public Guid SectionId { get; set; }

    [JsonPropertyName("section_title")]
    public string SectionTitle { get; set; } = string.Empty;

    [JsonPropertyName("total_questions")]
    public int TotalQuestions { get; set; }

    [JsonPropertyName("is_unlocked")]
    public bool IsUnlocked { get; set; }

    [JsonPropertyName("unlock_message")]
    public string? UnlockMessage { get; set; }

    [JsonPropertyName("quizzes")]
    public List<QuizDto> Quizzes { get; set; } = new();
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

public class SubmitSectionQuizRequest
{
    [Required]
    [JsonPropertyName("answers")]
    public List<SectionQuizAnswerInput> Answers { get; set; } = new();
}

public class SectionQuizAnswerResult
{
    [JsonPropertyName("quiz_id")]
    public Guid QuizId { get; set; }

    [JsonPropertyName("question")]
    public string Question { get; set; } = string.Empty;

    [JsonPropertyName("is_correct")]
    public bool IsCorrect { get; set; }

    [JsonPropertyName("selected_option_id")]
    public Guid SelectedOptionId { get; set; }

    [JsonPropertyName("correct_option_id")]
    public Guid CorrectOptionId { get; set; }

    [JsonPropertyName("explanation")]
    public string? Explanation { get; set; }
}

public class SubmitSectionQuizResponse
{
    [JsonPropertyName("attempt_id")]
    public Guid AttemptId { get; set; }

    [JsonPropertyName("score")]
    public decimal Score { get; set; }

    [JsonPropertyName("is_passed")]
    public bool IsPassed { get; set; }

    [JsonPropertyName("total_questions")]
    public int TotalQuestions { get; set; }

    [JsonPropertyName("correct_answers")]
    public int CorrectAnswers { get; set; }

    [JsonPropertyName("attempted_at")]
    public DateTime AttemptedAt { get; set; }

    [JsonPropertyName("answers")]
    public List<SectionQuizAnswerResult> Answers { get; set; } = new();

    [JsonPropertyName("certification_issued")]
    public bool CertificationIssued { get; set; }

    [JsonPropertyName("certificate_code")]
    public string? CertificateCode { get; set; }
}

public class SectionQuizAttemptDto
{
    [JsonPropertyName("attempt_id")]
    public Guid AttemptId { get; set; }

    [JsonPropertyName("section_id")]
    public Guid SectionId { get; set; }

    [JsonPropertyName("section_title")]
    public string SectionTitle { get; set; } = string.Empty;

    [JsonPropertyName("score")]
    public decimal Score { get; set; }

    [JsonPropertyName("is_passed")]
    public bool IsPassed { get; set; }

    [JsonPropertyName("attempted_at")]
    public DateTime AttemptedAt { get; set; }

    [JsonPropertyName("total_questions")]
    public int TotalQuestions { get; set; }

    [JsonPropertyName("correct_answers")]
    public int CorrectAnswers { get; set; }
}

public class SectionQuizAttemptListDto
{
    [JsonPropertyName("attempts")]
    public List<SectionQuizAttemptDto> Attempts { get; set; } = new();

    [JsonPropertyName("total_count")]
    public int TotalCount { get; set; }

    [JsonPropertyName("best_score")]
    public decimal? BestScore { get; set; }

    [JsonPropertyName("best_attempt_id")]
    public Guid? BestAttemptId { get; set; }
}
