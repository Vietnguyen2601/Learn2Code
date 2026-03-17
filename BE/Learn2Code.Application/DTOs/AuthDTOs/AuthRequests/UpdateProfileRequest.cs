using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Learn2Code.Application.DTOs.AuthDTOs.AuthRequests;

public class UpdateProfileRequest
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [Phone]
    [JsonPropertyName("phone_number")]
    public string? PhoneNumber { get; set; }
}
