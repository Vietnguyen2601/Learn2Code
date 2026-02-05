using System.ComponentModel.DataAnnotations.Schema;

namespace Learn2Code.Domain.Entities;

[Table("account_roles")]
public class AccountRole
{
    [Column("account_id")]
    public Guid AccountId { get; set; }

    [Column("role_id")]
    public Guid RoleId { get; set; }

    [Column("assigned_at")]
    public DateTime? AssignedAt { get; set; }

    // Navigation properties
    [ForeignKey("AccountId")]
    public virtual Account Account { get; set; } = null!;

    [ForeignKey("RoleId")]
    public virtual Role Role { get; set; } = null!;
}
