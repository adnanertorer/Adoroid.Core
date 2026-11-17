namespace Adoroid.Core.Repository.Repositories;

public abstract class AuditableEntity<TId> : BasicEntity<TId>
{
    public DateTimeOffset CreatedAt { get; protected set; } = DateTimeOffset.UtcNow;
    public Guid? CreatedBy { get; protected set; }
    public DateTimeOffset? UpdatedAt { get; protected set; }
    public Guid? UpdatedBy { get; protected set; }

    public void SetCreatedBy(Guid? userId)
    {
        CreatedBy = userId;
    }

    public void SetAuditCreated(DateTimeOffset createdAt, Guid? userId)
    {
        CreatedAt = createdAt.ToUniversalTime();
        CreatedBy = userId;
    }

    public void MarkAsUpdated(Guid? userId)
    {
        UpdatedAt = DateTimeOffset.UtcNow;
        UpdatedBy = userId;
    }

    public void SetAuditUpdated(DateTimeOffset updatedAt, Guid? userId)
    {
        UpdatedAt = updatedAt.ToUniversalTime();
        UpdatedBy = userId;
    }
}
