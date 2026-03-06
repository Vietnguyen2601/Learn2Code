using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Learn2Code.Application.DTOs;

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

    [JsonPropertyName("explanation")]
    public string? Explanation { get; set; }

    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; set; }

    [JsonPropertyName("updated_at")]
    public DateTime UpdatedAt { get; set; }

    [JsonPropertyName("options")]
    public List<QuizOptionDto> Options { get; set; } = new();
}

public class QuizOptionDto
{
    [JsonPropertyName("option_id")]
    public Guid OptionId { get; set; }

    [JsonPropertyName("quiz_id")]
    public Guid QuizId { get; set; }

    [JsonPropertyName("content")]
    public string Content { get; set; } = string.Empty;

    [JsonPropertyName("is_correct")]
    public bool IsCorrect { get; set; }

    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; set; }
}

public class CreateQuizRequest
{
    [Required]
    [JsonPropertyName("question")]
    public string Question { get; set; } = string.Empty;

    [JsonPropertyName("explanation")]
    public string? Explanation { get; set; }

    [Required]
    [MinLength(2, ErrorMessage = "Quiz must have at least 2 options")]
    [JsonPropertyName("options")]
    public List<CreateQuizOptionRequest> Options { get; set; } = new();
}

public class CreateQuizOptionRequest
{
    [Required]
    [JsonPropertyName("content")]
    public string Content { get; set; } = string.Empty;

    [JsonPropertyName("is_correct")]
    public bool IsCorrect { get; set; } = false;
}

public class UpdateQuizRequest
{
    [JsonPropertyName("question")]
    public string? Question { get; set; }

    [JsonPropertyName("explanation")]
    public string? Explanation { get; set; }

    [JsonPropertyName("order_number")]
    public int? OrderNumber { get; set; }

    [MinLength(2, ErrorMessage = "Quiz must have at least 2 options")]
    [JsonPropertyName("options")]
    public List<UpdateQuizOptionRequest>? Options { get; set; }
}

public class UpdateQuizOptionRequest
{
    [JsonPropertyName("option_id")]
    public Guid? OptionId { get; set; } // null = t?o m?i, có giá tr? = update

    [Required]
    [JsonPropertyName("content")]
    public string Content { get; set; } = string.Empty;

    [JsonPropertyName("is_correct")]
    public bool IsCorrect { get; set; } = false;
}

public class UpdateSingleQuizOptionRequest
{
    [Required]
    [JsonPropertyName("content")]
    public string Content { get; set; } = string.Empty;

    [JsonPropertyName("is_correct")]
    public bool IsCorrect { get; set; } = false;
}
