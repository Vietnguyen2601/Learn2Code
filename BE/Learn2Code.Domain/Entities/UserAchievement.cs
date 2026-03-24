using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Learn2Code.Domain.Entities;

/// <summary>
/// Ghi nhận thành tựu đã được unlock của từng học viên.
/// Unique constraint trên (user_id, achievement_id) đảm bảo mỗi achievement chỉ unlock 1 lần.
/// </summary>
[Table("user_achievements")]
public class UserAchievement
{
    [Key]
    [Column("userachievement_id")]
    public Guid UserAchievementId { get; set; }

    /// <summary>FK → accounts.account_id</summary>
    [Column("user_id")]
    public Guid UserId { get; set; }

    /// <summary>FK → achievements.achievement_id</summary>
    [Column("achievement_id")]
    public Guid AchievementId { get; set; }

    /// <summary>Thời điểm học viên unlock thành tựu này</summary>
    [Column("unlocked_at")]
    public DateTime UnlockedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    [ForeignKey("UserId")]
    public virtual Account User { get; set; } = null!;

    [ForeignKey("AchievementId")]
    public virtual Achievement Achievement { get; set; } = null!;
}
