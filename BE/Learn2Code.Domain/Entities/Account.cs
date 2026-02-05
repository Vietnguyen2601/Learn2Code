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

    // Navigation properties
    public virtual ICollection<AccountRole> AccountRoles { get; set; } = new List<AccountRole>();
    public virtual ICollection<Course> CreatedCourses { get; set; } = new List<Course>();
    public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    public virtual ICollection<Submission> Submissions { get; set; } = new List<Submission>();
    public virtual ICollection<QuizSubmission> QuizSubmissions { get; set; } = new List<QuizSubmission>();
    public virtual ICollection<Progress> Progresses { get; set; } = new List<Progress>();
    public virtual ICollection<Certification> Certifications { get; set; } = new List<Certification>();
    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();
}
