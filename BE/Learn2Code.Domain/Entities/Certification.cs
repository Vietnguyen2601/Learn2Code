using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Learn2Code.Domain.Entities;

[Table("certifications")]
public class Certification
{
    [Key]
    [Column("certification_id")]
    public Guid CertificationId { get; set; }

    [Column("student_id")]
    public Guid StudentId { get; set; }

    [Column("course_id")]
    public Guid CourseId { get; set; }

    [Required]
    [Column("certificate_code")]
    public string CertificateCode { get; set; } = string.Empty;

    [Column("certificate_url")]
    public string? CertificateUrl { get; set; }

    [Column("issued_at")]
    public DateTime IssuedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    [ForeignKey("StudentId")]
    public virtual Account Student { get; set; } = null!;

    [ForeignKey("CourseId")]
    public virtual Course Course { get; set; } = null!;
}
