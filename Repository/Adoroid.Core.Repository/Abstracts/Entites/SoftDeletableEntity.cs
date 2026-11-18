namespace Adoroid.Core.Repository.Abstracts.Entites;

public abstract class SoftDeletableEntity<TId> : AuditableEntity<TId>
{
    public bool IsDeleted { get; protected set; }
    public DateTimeOffset? DeletedAt { get; protected set; }
    public Guid? DeletedBy { get; protected set; }

    public void SoftDelete(Guid? userId)
    {
        if (IsDeleted)
        {
            return;
        }

        IsDeleted = true;
        DeletedAt = DateTimeOffset.UtcNow;
        DeletedBy = userId;
    }

    public void SetAuditDeleted(DateTimeOffset deletedAt, Guid? userId)
    {
        IsDeleted = true;
        DeletedAt = deletedAt.ToUniversalTime();
        DeletedBy = userId;
    }

    public void Restore(Guid? userId)
    {
        if (!IsDeleted)
        {
            return;
        }

        IsDeleted = false;
        DeletedAt = null;
        DeletedBy = null;
        MarkAsUpdated(userId);
    }
}