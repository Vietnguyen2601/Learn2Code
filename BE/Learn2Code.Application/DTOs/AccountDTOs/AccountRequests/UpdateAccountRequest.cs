using System.Text.Json.Serialization;

namespace Learn2Code.Application.DTOs.AccountDTOs.AccountRequests;

public class UpdateAccountRequest
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("phone_number")]
    public string? PhoneNumber { get; set; }

    [JsonPropertyName("is_active")]
    public bool? IsActive { get; set; }

    [JsonPropertyName("roles")]
    public List<string>? Roles { get; set; }
}
