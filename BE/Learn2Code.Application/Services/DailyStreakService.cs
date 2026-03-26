using Learn2Code.Application.Base;
using Learn2Code.Application.Constants;
using Learn2Code.Application.DTOs.GamificationDTOs;
using Learn2Code.Application.Interfaces;
using Learn2Code.Domain.Entities;
using Learn2Code.Infrastructure.Persistence.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Learn2Code.Application.Services;

public class DailyStreakService : IDailyStreakService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAchievementService _achievementService;
    private readonly ILogger<DailyStreakService> _logger;

    public DailyStreakService(
        IUnitOfWork unitOfWork,
        IAchievementService achievementService,
        ILogger<DailyStreakService> logger)
    {
        _unitOfWork = unitOfWork;
        _achievementService = achievementService;
        _logger = logger;
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Public: Lấy thông tin streak
    // ─────────────────────────────────────────────────────────────────────────

    public async Task<ServiceResult<DailyStreakDto>> GetUserStreakAsync(Guid userId)
    {
        try
        {
            // ── Load UserXP để lấy current / longest streak ──────────────────────
            var userXp = await _unitOfWork.Repository<UserXP>()
                .GetAsync(x => x.UserId == userId);

            if (userXp == null)
                return ServiceResult<DailyStreakDto>.NotFound("User XP not found");

            // ── Load lịch sử 30 ngày gần nhất ───────────────────────────────────
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var thirtyDaysAgo = today.AddDays(-29);

            var history = await _unitOfWork.Repository<DailyStreak>()
                .GetAllQueryable()
                .Where(ds => ds.UserId == userId && ds.StreakDate >= thirtyDaysAgo)
                .OrderByDescending(ds => ds.StreakDate)
                .ToListAsync();

            // ── Tính XP today ──────────────────────────────────────────────────
            var todayRecord = history.FirstOrDefault(ds => ds.StreakDate == today);
            int todayXP = todayRecord?.XPEarned ?? 0;

            // ── Transform lịch sử thành DTO ────────────────────────────────────
            // Cần tính "day count" cho từng ngày (thứ mấy trong streak)
            var historyDtos = TransformHistoryToDtos(history, today);

            var result = new DailyStreakDto
            {
                CurrentStreak = userXp.CurrentStreak,
                LongestStreak = userXp.LongestStreak,
                TodayXP = todayXP,
                StreakHistory = historyDtos,
                XPAvailableToday = todayRecord == null  // Nếu chưa check-in hôm nay
            };

            return ServiceResult<DailyStreakDto>.Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetUserStreakAsync failed for user {UserId}", userId);
            return ServiceResult<DailyStreakDto>.Error("GET_STREAK_FAILED", "Failed to get streak info", 500);
        }
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Check-in: Core streak logic
    // ─────────────────────────────────────────────────────────────────────────

    public async Task<ServiceResult<CheckInStreakResult>> CheckInAsync(Guid userId)
    {
        try
        {
            // ── 1. Load UserXP ────────────────────────────────────────────────────
            var userXp = await _unitOfWork.Repository<UserXP>()
                .GetAsync(x => x.UserId == userId);

            if (userXp == null)
                return ServiceResult<CheckInStreakResult>.NotFound("User XP not found");

            // ── 2. Xác định ngày hôm nay / hôm qua ─────────────────────────────────
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var yesterday = today.AddDays(-1);

            // ── 3. Kiểm tra đã check-in hôm nay chưa ──────────────────────────────
            var todayRecord = await _unitOfWork.Repository<DailyStreak>()
                .GetAsync(ds => ds.UserId == userId && ds.StreakDate == today);

            if (todayRecord != null)
            {
                // Đã check-in rồi, return early
                // (không award XP lại, prevent duplicate)
                _logger.LogInformation("User {UserId} already checked in today", userId);
                return ServiceResult<CheckInStreakResult>.Ok(new CheckInStreakResult
                {
                    Success = false,
                    Message = "Already checked in today",
                    CurrentStreak = userXp.CurrentStreak,
                    XPEarnedToday = todayRecord.XPEarned
                });
            }

            // ── 4. Tính streak: kiểm tra hôm qua ───────────────────────────────────
            var yesterdayRecord = await _unitOfWork.Repository<DailyStreak>()
                .GetAsync(ds => ds.UserId == userId && ds.StreakDate == yesterday);

            int newStreak = yesterdayRecord != null ? userXp.CurrentStreak + 1 : 1;

            // ── 5. Update longest streak nếu đạt mới ──────────────────────────────
            if (newStreak > userXp.LongestStreak)
                userXp.LongestStreak = newStreak;

            userXp.CurrentStreak = newStreak;

            // ── 6. Lookup XP thưởng theo streak ────────────────────────────────────
            int streakXPBonus = GamificationConstants.GetStreakBonus(newStreak);

            // ── 7. Tạo DailyStreak record ─────────────────────────────────────────
            var newRecord = new DailyStreak
            {
                StreakId = Guid.NewGuid(),
                UserId = userId,
                StreakDate = today,
                XPEarned = streakXPBonus,
                CreatedAt = DateTime.UtcNow
            };

            _unitOfWork.Repository<DailyStreak>().PrepareCreate(newRecord);

            // ── 8. Award XP bonus ──────────────────────────────────────────────────
            userXp.TotalXP += streakXPBonus;
            userXp.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.Repository<UserXP>().PrepareUpdate(userXp);

            // ── 9. Persist changes ────────────────────────────────────────────────
            await _unitOfWork.SaveChangesAsync();

            // ── 10. Log ────────────────────────────────────────────────────────────
            _logger.LogInformation(
                "User {UserId} checked in. Streak: {Streak}, XP Bonus: {XPBonus}",
                userId, newStreak, streakXPBonus);

            // ── 11. Kiểm tra milestone unlock ──────────────────────────────────────
            string? badgeUnlocked = null;
            if (newStreak == 7 || newStreak == 30 || newStreak == 100)
            {
                var achieveResult = await _achievementService.CheckAndUnlockAsync(
                    userId, GamificationConstants.AchievementConditionStreak);

                if (achieveResult.Success && achieveResult.Data?.NewAchievements.Any() == true)
                {
                    badgeUnlocked = string.Join(", ",
                        achieveResult.Data.NewAchievements.Select(a => a.Name));
                }
            }

            // ── 12. Return result ──────────────────────────────────────────────────
            return ServiceResult<CheckInStreakResult>.Ok(new CheckInStreakResult
            {
                Success = true,
                Message = "Check-in successful",
                CurrentStreak = newStreak,
                XPEarnedToday = streakXPBonus,
                NewBadgeUnlocked = badgeUnlocked,
                TotalXPBonusAwarded = streakXPBonus
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "CheckInAsync failed for user {UserId}", userId);
            return ServiceResult<CheckInStreakResult>.Error("CHECKIN_FAILED", "Failed to check in", 500);
        }
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Helper: Transform history
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Transform DailyStreak records thành HistoryItemDto.
    /// Tính "CurrentDayCount" = thứ mấy trong chuỗi streak liên tục.
    /// Ví dụ: Nếu có record hôm nay, hôm qua, 2 hôm trước → count là 3, 2, 1
    /// </summary>
    private List<DailyStreakHistoryItemDto> TransformHistoryToDtos(
        List<DailyStreak> history, DateOnly today)
    {
        var dtos = new List<DailyStreakHistoryItemDto>();

        // Sắp xếp từ hôm nay trở lại (DESC already done)
        int dayCount = 0;
        DateOnly? lastDate = null;

        foreach (var record in history)
        {
            // Kiểm tra liên tục: nếu gap > 1 ngày → reset counter
            if (lastDate.HasValue && (lastDate.Value.AddDays(-1) != record.StreakDate))
            {
                dayCount = 1;  // Reset (gap detected, nhưng vẫn count)
            }
            else
            {
                dayCount++;
            }

            dtos.Add(new DailyStreakHistoryItemDto
            {
                StreakDate = record.StreakDate,
                XPEarned = record.XPEarned,
                CurrentDayCount = dayCount
            });

            lastDate = record.StreakDate;
        }

        return dtos;
    }
}
