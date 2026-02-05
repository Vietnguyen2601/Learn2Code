using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Learn2Code.Domain.Entities;

[Table("lessons")]
public class Lesson
{
    [Key]
    [Column("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Column("section_id")]
    public int SectionId { get; set; }

    [Column("title")]
    public string? Title { get; set; }

    [Column("order_number")]
    public int? OrderNumber { get; set; }

    [Column("is_previewable")]
    public bool? IsPreviewable { get; set; }

    [Column("created_at")]
    public DateTime? CreatedAt { get; set; }

    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    [ForeignKey("SectionId")]
    public virtual Section Section { get; set; } = null!;

    public virtual ICollection<Document> Documents { get; set; } = new List<Document>();
    public virtual ICollection<Exercise> Exercises { get; set; } = new List<Exercise>();
    public virtual ICollection<Quiz> Quizzes { get; set; } = new List<Quiz>();
    public virtual ICollection<Progress> Progresses { get; set; } = new List<Progress>();
}
