namespace Learn2Code.Application.DTOs.GamificationDTOs;

public class GamificationResult
{
    public int XPGained { get; set; }
    public int StreakBonus { get; set; }
    public bool LevelUp { get; set; }
    public int NewLevel { get; set; }
    public int TotalXP { get; set; }
    public int StreakDays { get; set; }
    public List<string> NewBadges { get; set; } = new();
}
