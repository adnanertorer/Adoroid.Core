namespace Adoroid.Core.Repository.Abstracts.Entites.Tenancy;

public abstract class TenantEntity<TId> : SoftDeletableEntity<TId>
{
    public TId TenantId { get; protected set; }

    protected TenantEntity()
    {
        if(TenantId is null)
            throw new ArgumentNullException(nameof(TenantId), "TenantId cannot be null.");

        TenantId = default!;
    }

    protected TenantEntity(TId tenantId)
    {
        if (tenantId is null)
            throw new ArgumentNullException(nameof(tenantId), "TenantId cannot be null.");

        TenantId = tenantId;
    }
}