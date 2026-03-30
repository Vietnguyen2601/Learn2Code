using System.Text.Json.Serialization;

namespace Learn2Code.Application.DTOs.CertificationDTOs.CertificationResponses;

/// <summary>
/// Course completion requirements
/// </summary>
public class CertificationRequirementsDto
{
    [JsonPropertyName("min_weight_score")]
    public decimal MinWeightScore { get; set; }
}
