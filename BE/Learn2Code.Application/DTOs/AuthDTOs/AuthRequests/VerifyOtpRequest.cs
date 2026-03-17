using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Learn2Code.Application.DTOs.AuthDTOs.AuthRequests;

public class VerifyOtpRequest
{
    [Required]
    [EmailAddress]
    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;

    [Required]
    [JsonPropertyName("otp_code")]
    public string OtpCode { get; set; } = string.Empty;
}
