using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Learn2Code.Domain.Entities;

[Table("accounts")]
public class Account
{
    [Key]
    [Column("account_id")]
    public Guid AccountId { get; set; }

    [Required]
    [Column("username")]
    public string Username { get; set; } = string.Empty;

    [Required]
    [Column("password")]
    public string Password { get; set; } = string.Empty;

    [Required]
    [Column("email")]
    public string Email { get; set; } = string.Empty;

    [Column("name")]
    public string? Name { get; set; }

    [Column("phone_number")]
    public string? PhoneNumber { get; set; }

    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    [Column("created_at")]
    public DateTime? CreatedAt { get; set; }

    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }

    [Column("refresh_token")]
    public string? RefreshToken { get; set; }

    [Column("refresh_token_expiry")]
    public DateTime? RefreshTokenExpiry { get; set; }

    // Navigation properties
    public virtual ICollection<AccountRole> AccountRoles { get; set; } = new List<AccountRole>();
    public virtual ICollection<Course> CreatedCourses { get; set; } = new List<Course>();
    public virtual ICollection<UserSubscription> UserSubscriptions { get; set; } = new List<UserSubscription>();
    public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    public virtual ICollection<LessonProgress> LessonProgresses { get; set; } = new List<LessonProgress>();
    public virtual ICollection<ExerciseProgress> ExerciseProgresses { get; set; } = new List<ExerciseProgress>();
    public virtual ICollection<SectionQuizAttempt> SectionQuizAttempts { get; set; } = new List<SectionQuizAttempt>();
    public virtual ICollection<Certification> Certifications { get; set; } = new List<Certification>();
    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();
    public virtual ICollection<Leaderboard> Leaderboards { get; set; } = new List<Leaderboard>();
}
