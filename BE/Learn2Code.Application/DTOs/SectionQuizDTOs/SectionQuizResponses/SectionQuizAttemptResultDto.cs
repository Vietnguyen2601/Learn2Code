using System.Text.Json.Serialization;

namespace Learn2Code.Application.DTOs.SectionQuizDTOs.SectionQuizResponses;

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

    /// <summary>
    /// Certification result if certificate was issued after this attempt
    /// </summary>
    [JsonPropertyName("certification_result")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public object? CertificationResult { get; set; }
}
