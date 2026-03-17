using System.Text.Json.Serialization;

namespace Learn2Code.Application.DTOs.CertificationDTOs.CertificationResponses;

/// <summary>
/// Response for checking certification eligibility
/// </summary>
public class CertificationEligibilityDto
{
    [JsonPropertyName("is_eligible")]
    public bool IsEligible { get; set; }

    [JsonPropertyName("already_certified")]
    public bool AlreadyCertified { get; set; }

    [JsonPropertyName("existing_certificate_code")]
    public string? ExistingCertificateCode { get; set; }

    [JsonPropertyName("progress")]
    public CertificationProgressDto Progress { get; set; } = new();

    [JsonPropertyName("requirements")]
    public CertificationRequirementsDto Requirements { get; set; } = new();

    [JsonPropertyName("missing")]
    public List<string> Missing { get; set; } = new();
}
