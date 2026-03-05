using System.ComponentModel.DataAnnotations.Schema;

namespace Learn2Code.Application.Models;

public class OtpVerification
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Email { get; set; } = string.Empty;
    public string OtpCode { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public bool IsUsed { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
