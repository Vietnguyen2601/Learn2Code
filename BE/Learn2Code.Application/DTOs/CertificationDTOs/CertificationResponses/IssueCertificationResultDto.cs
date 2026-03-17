using System.Text.Json.Serialization;

namespace Learn2Code.Application.DTOs.CertificationDTOs.CertificationResponses;

/// <summary>
/// Response after issuing a certificate
/// </summary>
public class IssueCertificationResultDto
{
    [JsonPropertyName("certified")]
    public bool Certified { get; set; }

    [JsonPropertyName("certificate_code")]
    public string? CertificateCode { get; set; }

    [JsonPropertyName("certificate_url")]
    public string? CertificateUrl { get; set; }

    [JsonPropertyName("message")]
    public string? Message { get; set; }

    [JsonPropertyName("missing")]
    public List<string>? Missing { get; set; }
}
