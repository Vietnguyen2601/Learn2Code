using System.Text.Json.Serialization;

namespace Learn2Code.Application.DTOs.AuthDTOs.AuthResponses;

public class MeResponse
{
    [JsonPropertyName("account_id")]
    public Guid AccountId { get; set; }

    [JsonPropertyName("username")]
    public string Username { get; set; } = string.Empty;

    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("phone_number")]
    public string? PhoneNumber { get; set; }

    [JsonPropertyName("is_active")]
    public bool IsActive { get; set; }

    [JsonPropertyName("roles")]
    public List<string> Roles { get; set; } = new();

    [JsonPropertyName("created_at")]
    public DateTime? CreatedAt { get; set; }
}
