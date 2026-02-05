using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Learn2Code.Application.DTOs;

public class AccountDto
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

    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; set; }

    [JsonPropertyName("roles")]
    public List<string> Roles { get; set; } = new();
}

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
