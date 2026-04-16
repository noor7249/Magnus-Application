using Magnus.API.Models.Interfaces;

namespace Magnus.API.Models.Base;

public abstract class AuditableEntity : IAuditableEntity, ISoftDeletable
{
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }
}
