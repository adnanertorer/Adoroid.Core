using Adoroid.Core.Repository.Tenancy;

namespace Adoroid.Core.Repository.Abstracts.Entites.Tenancy;

/// <typeparam name="TId">Entity Id tipi.</typeparam>
/// <typeparam name="TTenantId">Tenant Id tipi (entity Id tipinden bağımsız).</typeparam>
public abstract class TenantEntity<TId, TTenantId> : SoftDeletableEntity<TId>, ITenantEntity<TTenantId>, ITenantAssignable<TTenantId>
{
    public TTenantId TenantId { get; protected set; } = default!;

    // EF Core materialization için. TenantId burada kontrol edilmez, veritabanından gelir.
    protected TenantEntity()
    {
    }

    protected TenantEntity(TTenantId tenantId)
    {
        if (TenantGuard.IsEmpty(tenantId))
            throw new ArgumentException("TenantId cannot be empty.", nameof(tenantId));

        TenantId = tenantId;
    }

    void ITenantAssignable<TTenantId>.AssignTenant(TTenantId tenantId)
    {
        if (TenantGuard.IsEmpty(tenantId))
            throw new ArgumentException("TenantId cannot be empty.", nameof(tenantId));

        if (!TenantGuard.IsEmpty(TenantId) && !TenantGuard.AreEqual(TenantId, tenantId))
            throw new TenantAccessException("Entity already belongs to another tenant.");

        TenantId = tenantId;
    }
}
