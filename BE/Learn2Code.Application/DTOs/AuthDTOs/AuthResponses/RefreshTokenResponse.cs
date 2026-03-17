using System.Text.Json.Serialization;

namespace Learn2Code.Application.DTOs.AuthDTOs.AuthResponses;

public class RefreshTokenResponse
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; } = string.Empty;

    [JsonPropertyName("refresh_token")]
    public string RefreshToken { get; set; } = string.Empty;
}
