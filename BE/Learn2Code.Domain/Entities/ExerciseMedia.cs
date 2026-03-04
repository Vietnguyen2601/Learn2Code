using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Learn2Code.Domain.Enums;

namespace Learn2Code.Domain.Entities;

[Table("exercise_media")]
public class ExerciseMedia
{
    [Key]
    [Column("media_id")]
    public Guid MediaId { get; set; }

    [Column("exercise_id")]
    public Guid ExerciseId { get; set; }

    [Column("media_type")]
    public MediaType MediaType { get; set; }

    [Required]
    [Column("url")]
    public string Url { get; set; } = string.Empty;

    [Column("caption")]
    public string? Caption { get; set; }

    [Column("order_number")]
    public int OrderNumber { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    [ForeignKey("ExerciseId")]
    public virtual Exercise Exercise { get; set; } = null!;
}
