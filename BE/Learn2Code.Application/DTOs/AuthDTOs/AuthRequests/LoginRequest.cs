using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Learn2Code.Application.DTOs.AuthDTOs.AuthRequests;

public class LoginRequest
{
    [Required]
    [JsonPropertyName("email_or_username")]
    public string EmailOrUsername { get; set; } = string.Empty;

    [Required]
    [JsonPropertyName("password")]
    public string Password { get; set; } = string.Empty;
}
