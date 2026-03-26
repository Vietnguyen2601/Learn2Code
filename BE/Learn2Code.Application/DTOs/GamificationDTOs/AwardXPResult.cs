namespace Learn2Code.Application.DTOs.GamificationDTOs;

public class AwardXPResult
{
    public int XPGained { get; set; }
    public int StreakBonus { get; set; }
    public int NewTotalXP { get; set; }
    public int CurrentLevel { get; set; }
    public bool LevelUp { get; set; }
    public int XpToNext { get; set; }
    public int StreakDays { get; set; }
}
