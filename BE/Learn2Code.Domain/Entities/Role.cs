using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Learn2Code.Domain.Entities;

[Table("roles")]
public class Role
{
    [Key]
    [Column("role_id")]
    public Guid RoleId { get; set; }

    [Required]
    [Column("role_name")]
    public string RoleName { get; set; } = string.Empty;

    [Column("description")]
    public string? Description { get; set; }

    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    [Column("created_at")]
    public DateTime? CreatedAt { get; set; }

    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public virtual ICollection<AccountRole> AccountRoles { get; set; } = new List<AccountRole>();
}
