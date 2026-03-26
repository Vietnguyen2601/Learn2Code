namespace Learn2Code.Application.DTOs.AchievementDTOs;

public class UpdateAchievementRequest
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? IconUrl { get; set; }
    public int? XPReward { get; set; }
    public string? ConditionType { get; set; }
    public int? ConditionValue { get; set; }
    public bool? IsHidden { get; set; }
}
