namespace Learn2Code.Application.DTOs.AchievementDTOs;

public class AchievementDto
{
    public Guid AchievementId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? IconUrl { get; set; }
    public int XPReward { get; set; }
    public string? ConditionType { get; set; }
    public int? ConditionValue { get; set; }
    public bool IsHidden { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
