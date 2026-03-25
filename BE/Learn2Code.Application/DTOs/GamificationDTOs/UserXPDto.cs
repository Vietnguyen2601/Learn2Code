namespace Learn2Code.Application.DTOs.GamificationDTOs;

public class UserXPDto
{
    public Guid UserId { get; set; }
    public int TotalXP { get; set; }
    public int CurrentLevel { get; set; }
    public int XPToNext { get; set; }
    public int CurrentStreak { get; set; }
    public int LongestStreak { get; set; }
    public DateTime UpdatedAt { get; set; }
}
