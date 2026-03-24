namespace Learn2Code.Domain.Enums;

public enum CourseDifficulty
{
    Beginner,
    Intermediate,
    Advanced
}

public enum EnrollmentStatus
{
    Enrolled,
    InProgress,
    Completed
}

public enum PaymentMethod
{
    PayOS
}

public enum PaymentStatus
{
    Pending,
    Success,
    Failed
}

public enum ExerciseType
{
    Reading,
    FreeCode,
    GradedCode
}

public enum SubscriptionStatus
{
    Pending,
    Active,
    Expired,
    Cancelled
}

public enum MediaType
{
    Image,
    Video
}

public enum LessonProgressStatus
{
    NotStarted,
    InProgress,
    Completed
}

/// <summary>
/// Loại sự kiện học tập kích hoạt việc tính XP trong hệ thống Gamification.
/// </summary>
public enum XPEventType
{
    /// <summary>Hoàn thành một bài học</summary>
    LessonCompleted,

    /// <summary>Vượt qua bài tập code (is_passed = true)</summary>
    ExercisePassed,

    /// <summary>Trả lời đúng 100% câu hỏi quiz trong một section</summary>
    QuizPerfect,

    /// <summary>Hoàn thành toàn bộ khoá học</summary>
    CourseCompleted,

    /// <summary>Duy trì streak học liên tiếp mỗi ngày</summary>
    DailyStreak,

    /// <summary>Đăng nhập lần đầu tiên</summary>
    FirstLogin
}
