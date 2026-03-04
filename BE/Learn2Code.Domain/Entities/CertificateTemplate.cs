using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Learn2Code.Domain.Entities;

[Table("certificate_templates")]
public class CertificateTemplate
{
    [Key]
    [Column("template_id")]
    public Guid TemplateId { get; set; }

    [Column("course_id")]
    public Guid CourseId { get; set; }

    [Required]
    [Column("title")]
    public string Title { get; set; } = string.Empty;

    [Column("description")]
    public string? Description { get; set; }

    [Column("background_image_url")]
    public string? BackgroundImageUrl { get; set; }

    [Column("signature_name")]
    public string? SignatureName { get; set; }

    [Column("signature_image_url")]
    public string? SignatureImageUrl { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    [ForeignKey("CourseId")]
    public virtual Course Course { get; set; } = null!;
}
