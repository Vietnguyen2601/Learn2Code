using Learn2Code.Application.Base;
using Learn2Code.Application.Constants;
using Learn2Code.Application.DTOs.GamificationDTOs;
using Learn2Code.Application.Interfaces;
using Learn2Code.Domain.Entities;
using Learn2Code.Domain.Enums;
using Learn2Code.Infrastructure.Persistence.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Learn2Code.Application.Services;

public class GamificationService : IGamificationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GamificationService> _logger;

    public GamificationService(IUnitOfWork unitOfWork, ILogger<GamificationService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<GamificationResult> ProcessEventAsync(Guid userId, XPEventType eventType)
    {
        try
        {
            var baseXP = GamificationConstants.XPRewards.GetValueOrDefault(eventType, 0);
            var result = new GamificationResult { XPGained = baseXP };

            // ── 1. Get or create UserXP record ──────────────────────────────
            var userXp = await _unitOfWork.Repository<UserXP>()
                .GetAsync(x => x.UserId == userId);

            var isNew = userXp == null;
            if (isNew)
            {
                userXp = new UserXP
                {
                    UserXPId = Guid.NewGuid(),
                    UserId = userId,
                    TotalXP = 0,
                    CurrentLevel = 1,
                    XPToNext = GamificationConstants.BaseXpPerLevel,
                    CurrentStreak = 0,
                    LongestStreak = 0,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
            }

            var prevLevel = userXp!.CurrentLevel;

            // ── 2. Daily streak handling ─────────────────────────────────────
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var streakBonus = 0;

            var todayStreak = await _unitOfWork.Repository<DailyStreak>()
                .GetAsync(ds => ds.UserId == userId && ds.StreakDate == today);

            if (todayStreak == null)
            {
                // First activity today → check if yesterday has a record to continue streak
                var yesterday = today.AddDays(-1);
                var hadActivityYesterday = await _unitOfWork.Repository<DailyStreak>()
                    .GetAllQueryable()
                    .AnyAsync(ds => ds.UserId == userId && ds.StreakDate == yesterday);

                userXp.CurrentStreak = hadActivityYesterday ? userXp.CurrentStreak + 1 : 1;
                if (userXp.CurrentStreak > userXp.LongestStreak)
                    userXp.LongestStreak = userXp.CurrentStreak;

                // Award streak bonus XP
                streakBonus = GamificationConstants.GetStreakBonus(userXp.CurrentStreak);
                result.StreakBonus = streakBonus;

                // Create DailyStreak record
                var newStreak = new DailyStreak
                {
                    StreakId = Guid.NewGuid(),
                    UserId = userId,
                    StreakDate = today,
                    XPEarned = baseXP + streakBonus,
                    CreatedAt = DateTime.UtcNow
                };
                _unitOfWork.Repository<DailyStreak>().PrepareCreate(newStreak);
            }
            else
            {
                // Already logged today; just accumulate XP on the existing record
                todayStreak.XPEarned += baseXP;
                _unitOfWork.Repository<DailyStreak>().PrepareUpdate(todayStreak);
            }

            result.StreakDays = userXp.CurrentStreak;

            // ── 3. Award XP ──────────────────────────────────────────────────
            userXp.TotalXP += baseXP + streakBonus;
            userXp.CurrentLevel = GamificationConstants.CalculateLevel(userXp.TotalXP);
            userXp.XPToNext = GamificationConstants.XpToNextLevel(userXp.TotalXP);
            userXp.UpdatedAt = DateTime.UtcNow;

            if (isNew)
                _unitOfWork.Repository<UserXP>().PrepareCreate(userXp);
            else
                _unitOfWork.Repository<UserXP>().PrepareUpdate(userXp);

            result.TotalXP = userXp.TotalXP;
            result.NewLevel = userXp.CurrentLevel;
            result.LevelUp = userXp.CurrentLevel > prevLevel;

            // ── 4. Achievement check ─────────────────────────────────────────
            result.NewBadges = await CheckAndUnlockAchievementsAsync(userId, userXp);

            // ── 5. Persist ───────────────────────────────────────────────────
            await _unitOfWork.SaveChangesAsync();

            return result;
        }
        catch (Exception ex)
        {
            // Gamification must never crash the main flow
            _logger.LogError(ex, "GamificationService.ProcessEventAsync failed for user {UserId}, event {Event}", userId, eventType);
            return new GamificationResult();
        }
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Private helpers
    // ─────────────────────────────────────────────────────────────────────────

    private async Task<List<string>> CheckAndUnlockAchievementsAsync(Guid userId, UserXP userXp)
    {
        var newBadges = new List<string>();

        // Load all achievements the user hasn't unlocked yet
        var unlockedIds = await _unitOfWork.Repository<UserAchievement>()
            .GetAllQueryable()
            .Where(ua => ua.UserId == userId)
            .Select(ua => ua.AchievementId)
            .ToListAsync();

        var pending = await _unitOfWork.Repository<Achievement>()
            .GetAllQueryable()
            .Where(a => !unlockedIds.Contains(a.AchievementId)
                        && a.ConditionType != null
                        && a.ConditionValue != null)
            .ToListAsync();

        if (!pending.Any()) return newBadges;

        // Pre-fetch counts only if there are achievements that need them
        int? lessonCount = pending.Any(a => a.ConditionType == GamificationConstants.AchievementConditionLessons)
            ? await _unitOfWork.Repository<LessonProgress>().GetAllQueryable()
                .CountAsync(lp => lp.StudentId == userId && lp.Status == LessonProgressStatus.Completed)
            : (int?)null;

        int? exerciseCount = pending.Any(a => a.ConditionType == GamificationConstants.AchievementConditionExercises)
            ? await _unitOfWork.Repository<ExerciseProgress>().GetAllQueryable()
                .CountAsync(ep => ep.StudentId == userId && ep.IsPassed)
            : (int?)null;

        foreach (var achievement in pending)
        {
            var conditionMet = achievement.ConditionType switch
            {
                GamificationConstants.AchievementConditionXP => userXp.TotalXP >= achievement.ConditionValue,
                GamificationConstants.AchievementConditionLevel => userXp.CurrentLevel >= achievement.ConditionValue,
                GamificationConstants.AchievementConditionStreak => userXp.CurrentStreak >= achievement.ConditionValue,
                GamificationConstants.AchievementConditionLessons => lessonCount >= achievement.ConditionValue,
                GamificationConstants.AchievementConditionExercises => exerciseCount >= achievement.ConditionValue,
                _ => false
            };

            if (conditionMet != true) continue;

            var unlock = new UserAchievement
            {
                UserAchievementId = Guid.NewGuid(),
                UserId = userId,
                AchievementId = achievement.AchievementId,
                UnlockedAt = DateTime.UtcNow
            };
            _unitOfWork.Repository<UserAchievement>().PrepareCreate(unlock);

            // Award bonus XP for the achievement
            if (achievement.XPReward > 0)
                userXp.TotalXP += achievement.XPReward;

            newBadges.Add(achievement.Name);
        }

        return newBadges;
    }
}
