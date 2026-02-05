using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Learn2Code.Domain.Enums;

namespace Learn2Code.Domain.Entities;

[Table("documents")]
public class Document
{
    [Key]
    [Column("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Column("lesson_id")]
    public int LessonId { get; set; }

    [Column("title")]
    public string? Title { get; set; }

    [Column("content")]
    public string? Content { get; set; }

    [Column("doc_type")]
    public DocumentType? DocType { get; set; }

    [Column("order_number")]
    public int? OrderNumber { get; set; }

    [Column("created_at")]
    public DateTime? CreatedAt { get; set; }

    // Navigation properties
    [ForeignKey("LessonId")]
    public virtual Lesson Lesson { get; set; } = null!;
}
