using System.Text.Json.Serialization;

namespace Learn2Code.Application.DTOs.AdminDTOs.AdminRequests;

public class UpdateUserRequest
{
    [JsonPropertyName("role")]
    public string? Role { get; set; }

    [JsonPropertyName("is_active")]
    public bool? IsActive { get; set; }
}
