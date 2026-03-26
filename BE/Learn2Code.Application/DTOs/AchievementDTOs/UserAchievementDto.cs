namespace Learn2Code.Application.DTOs.AchievementDTOs;

public class UserAchievementDto
{
    public Guid UserAchievementId { get; set; }
    public Guid AchievementId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? IconUrl { get; set; }
    public int XPReward { get; set; }
    public DateTime UnlockedAt { get; set; }
}
