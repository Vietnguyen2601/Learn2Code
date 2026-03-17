using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Learn2Code.Application.DTOs.AuthDTOs.AuthRequests;

public class ResetPasswordRequest
{
    [Required]
    [EmailAddress]
    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;

    [Required]
    [JsonPropertyName("otp_code")]
    public string OtpCode { get; set; } = string.Empty;

    [Required]
    [MinLength(6)]
    [JsonPropertyName("new_password")]
    public string NewPassword { get; set; } = string.Empty;

    [Required]
    [Compare("NewPassword")]
    [JsonPropertyName("confirm_password")]
    public string ConfirmPassword { get; set; } = string.Empty;
}
