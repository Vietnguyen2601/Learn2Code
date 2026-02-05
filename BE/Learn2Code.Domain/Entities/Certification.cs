using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Learn2Code.Domain.Entities;

[Table("certifications")]
public class Certification
{
    [Key]
    [Column("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Column("student_id")]
    public Guid StudentId { get; set; }

    [Column("course_id")]
    public int CourseId { get; set; }

    [Column("issue_date")]
    public DateTime? IssueDate { get; set; }

    [Column("certificate_code")]
    public string? CertificateCode { get; set; }

    [Column("certificate_url")]
    public string? CertificateUrl { get; set; }

    // Navigation properties
    [ForeignKey("StudentId")]
    public virtual Account Student { get; set; } = null!;

    [ForeignKey("CourseId")]
    public virtual Course Course { get; set; } = null!;
}
