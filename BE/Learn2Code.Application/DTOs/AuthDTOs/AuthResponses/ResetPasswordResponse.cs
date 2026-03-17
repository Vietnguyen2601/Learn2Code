using System.Text.Json.Serialization;

namespace Learn2Code.Application.DTOs.AuthDTOs.AuthResponses;

public class ResetPasswordResponse
{
    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;

    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;
}
