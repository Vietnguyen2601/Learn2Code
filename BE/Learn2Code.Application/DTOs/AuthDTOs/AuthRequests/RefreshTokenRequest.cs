using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Learn2Code.Application.DTOs.AuthDTOs.AuthRequests;

public class RefreshTokenRequest
{
    [Required]
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; } = string.Empty;

    [Required]
    [JsonPropertyName("refresh_token")]
    public string RefreshToken { get; set; } = string.Empty;
}
