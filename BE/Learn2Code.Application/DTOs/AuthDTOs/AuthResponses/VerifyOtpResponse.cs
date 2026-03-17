using System.Text.Json.Serialization;

namespace Learn2Code.Application.DTOs.AuthDTOs.AuthResponses;

public class VerifyOtpResponse
{
    [JsonPropertyName("username")]
    public string Username { get; set; } = string.Empty;

    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string? Name { get; set; }
}
