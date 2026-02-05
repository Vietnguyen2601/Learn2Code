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
    Completed,
    Dropped
}

public enum PaymentMethod
{
    VNPay,
    BankTransfer,
    Free
}

public enum PaymentStatus
{
    Pending,
    Success,
    Failed,
    Refunded
}

public enum DocumentType
{
    Theory,
    ExerciseDescription
}

public enum ExerciseDifficulty
{
    Easy,
    Medium,
    Hard
}

public enum ProgrammingLanguage
{
    Python,
    JavaScript,
    Java,
    CSharp,
    CPlusPlus,
    Go,
    Rust,
    Other
}

public enum SubmissionStatus
{
    Pending,
    Accepted,
    WrongAnswer,
    RuntimeError
}

public enum ProgressStatus
{
    NotStarted,
    InProgress,
    Completed
}
