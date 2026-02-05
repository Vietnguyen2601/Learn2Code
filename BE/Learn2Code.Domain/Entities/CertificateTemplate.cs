using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Learn2Code.Domain.Entities;

[Table("certificate_templates")]
public class CertificateTemplate
{
    [Key]
    [Column("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Column("course_id")]
    public int CourseId { get; set; }

    [Column("title")]
    public string? Title { get; set; }

    [Column("description")]
    public string? Description { get; set; }

    [Column("background_image_url")]
    public string? BackgroundImageUrl { get; set; }

    [Column("signature_name")]
    public string? SignatureName { get; set; }

    [Column("signature_image_url")]
    public string? SignatureImageUrl { get; set; }

    [Column("created_at")]
    public DateTime? CreatedAt { get; set; }

    // Navigation properties
    [ForeignKey("CourseId")]
    public virtual Course Course { get; set; } = null!;
}
