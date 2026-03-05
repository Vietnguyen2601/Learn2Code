using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Learn2Code.Domain.Enums;

namespace Learn2Code.Domain.Entities;

[Table("courses")]
public class Course
{
    [Key]
    [Column("course_id")]
    public Guid CourseId { get; set; }

    [Required]
    [Column("title")]
    public string Title { get; set; } = string.Empty;

    [Column("description")]
    public string? Description { get; set; }

    [Column("difficulty")]
    public CourseDifficulty? Difficulty { get; set; }

    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    [Column("instructor_id")]
    public Guid InstructorId { get; set; }

    [Column("category_id")]
    public Guid? CategoryId { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    [ForeignKey("InstructorId")]
    public virtual Account? Instructor { get; set; }

    [ForeignKey("CategoryId")]
    public virtual CourseCategory? Category { get; set; }

    public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    public virtual ICollection<Section> Sections { get; set; } = new List<Section>();
    public virtual CertificateTemplate? CertificateTemplate { get; set; }
    public virtual ICollection<Certification> Certifications { get; set; } = new List<Certification>();
    public virtual CourseCompletionRule? CompletionRule { get; set; }
    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();
    public virtual ICollection<Leaderboard> Leaderboards { get; set; } = new List<Leaderboard>();
}
