using Learn2Code.Domain.Enums;

namespace Learn2Code.Application.Constants;

public static class GamificationConstants
{
    // XP awarded per event type
    public static readonly Dictionary<XPEventType, int> XPRewards = new()
    {
        { XPEventType.LessonCompleted,  50  },
        { XPEventType.ExercisePassed,   80  },
        { XPEventType.QuizPerfect,      120 },
        { XPEventType.CourseCompleted,  500 },
        { XPEventType.DailyStreak,      20  },
        { XPEventType.FirstLogin,       10  },
    };

    // Level formula: level = floor(sqrt(totalXp / BaseXpPerLevel)) + 1
    public const int BaseXpPerLevel = 100;

    public static int CalculateLevel(int totalXp)
    {
        if (totalXp <= 0) return 1;
        return (int)Math.Floor(Math.Sqrt((double)totalXp / BaseXpPerLevel)) + 1;
    }

    public static int XpToNextLevel(int totalXp)
    {
        var currentLevel = CalculateLevel(totalXp);
        var xpNeededForNextLevel = currentLevel * currentLevel * BaseXpPerLevel;
        return Math.Max(0, xpNeededForNextLevel - totalXp);
    }

    // Streak bonus XP thresholds (day count → bonus XP)
    public static readonly (int Day, int Bonus)[] StreakBonuses =
    [
        (3,  10),
        (7,  30),
        (14, 60),
        (30, 100),
    ];

    public static int GetStreakBonus(int streakDays)
    {
        var bonus = 0;
        foreach (var (day, xp) in StreakBonuses)
        {
            if (streakDays >= day)
                bonus = xp; // take the highest qualifying tier
        }
        return bonus;
    }

    // Achievement condition types
    public const string AchievementConditionLessons = "lessons_completed";
    public const string AchievementConditionExercises = "exercises_passed";
    public const string AchievementConditionStreak = "streak_days";
    public const string AchievementConditionXP = "total_xp";
    public const string AchievementConditionLevel = "level_reached";
}
