using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Learn2Code.Application.DTOs.AccountDTOs.AccountRequests;

public class CreateAccountRequest
{
    [Required]
    [JsonPropertyName("username")]
    public string Username { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MinLength(6)]
    [JsonPropertyName("password")]
    public string Password { get; set; } = string.Empty;

    [Required]
    [Compare("Password")]
    [JsonPropertyName("confirm_password")]
    public string ConfirmPassword { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("phone_number")]
    public string? PhoneNumber { get; set; }

    [JsonPropertyName("roles")]
    public List<string>? Roles { get; set; }
}
