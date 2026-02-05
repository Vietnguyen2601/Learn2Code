using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Learn2Code.Domain.Enums;

namespace Learn2Code.Domain.Entities;

[Table("courses")]
public class Course
{
    [Key]
    [Column("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [Column("title")]
    public string Title { get; set; } = string.Empty;

    [Column("description")]
    public string? Description { get; set; }

    [Column("price")]
    public decimal? Price { get; set; }

    [Column("original_price")]
    public decimal? OriginalPrice { get; set; }

    [Column("difficulty")]
    public CourseDifficulty? Difficulty { get; set; }

    [Column("thumbnail_url")]
    public string? ThumbnailUrl { get; set; }

    [Column("is_published")]
    public bool IsPublished { get; set; } = false;

    [Column("admin_id")]
    public Guid? AdminId { get; set; }

    [Column("category_id")]
    public int? CategoryId { get; set; }

    [Column("created_at")]
    public DateTime? CreatedAt { get; set; }

    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    [ForeignKey("AdminId")]
    public virtual Account? Admin { get; set; }

    [ForeignKey("CategoryId")]
    public virtual CourseCategory? Category { get; set; }

    public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    public virtual ICollection<Section> Sections { get; set; } = new List<Section>();
    public virtual FinalTest? FinalTest { get; set; }
    public virtual CertificateTemplate? CertificateTemplate { get; set; }
    public virtual ICollection<Certification> Certifications { get; set; } = new List<Certification>();
    public virtual CourseCompletionRule? CompletionRule { get; set; }
    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();
}
