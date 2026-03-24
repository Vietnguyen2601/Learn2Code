using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Learn2Code.Domain.Entities;

/// <summary>
/// Ghi nhận mỗi ngày học viên có hoạt động học tập.
/// Unique constraint trên (user_id, streak_date) đảm bảo mỗi ngày chỉ có 1 record.
/// Streak count hiện tại được lưu tại UserXP.CurrentStreak để tránh query tính toán.
/// </summary>
[Table("daily_streaks")]
public class DailyStreak
{
    [Key]
    [Column("streak_id")]
    public Guid StreakId { get; set; }

    /// <summary>FK → accounts.account_id</summary>
    [Column("user_id")]
    public Guid UserId { get; set; }

    /// <summary>Ngày học (kiểu date, không có giờ phút giây)</summary>
    [Column("streak_date")]
    public DateOnly StreakDate { get; set; }

    /// <summary>Tổng XP kiếm được trong ngày này</summary>
    [Column("xp_earned")]
    public int XPEarned { get; set; } = 0;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    [ForeignKey("UserId")]
    public virtual Account User { get; set; } = null!;
}
