using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Learn2Code.Domain.Entities;

/// <summary>
/// Lưu trữ điểm kinh nghiệm (XP) và level của từng học viên.
/// Mỗi user chỉ có đúng 1 record trong bảng này (unique index trên user_id).
/// </summary>
[Table("user_xp")]
public class UserXP
{
    [Key]
    [Column("userxp_id")]
    public Guid UserXPId { get; set; }

    /// <summary>FK → accounts.account_id</summary>
    [Column("user_id")]
    public Guid UserId { get; set; }

    /// <summary>Tổng XP đã tích lũy từ trước đến nay</summary>
    [Column("total_xp")]
    public int TotalXP { get; set; } = 0;

    /// <summary>Level hiện tại của học viên (bắt đầu từ 1)</summary>
    [Column("current_level")]
    public int CurrentLevel { get; set; } = 1;

    /// <summary>XP cần thêm để lên level tiếp theo</summary>
    [Column("xp_to_next")]
    public int XPToNext { get; set; } = 100;

    /// <summary>Số ngày học liên tiếp hiện tại</summary>
    [Column("current_streak")]
    public int CurrentStreak { get; set; } = 0;

    /// <summary>Chuỗi ngày học dài nhất từ trước đến nay</summary>
    [Column("longest_streak")]
    public int LongestStreak { get; set; } = 0;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    [ForeignKey("UserId")]
    public virtual Account User { get; set; } = null!;
}
