namespace Adoroid.Core.Repository.Tenancy;

/// <summary>
/// Global tenant query filter'ı için DbContext tarafından uygulanır.
/// Değer her sorguda yeniden okunur; ör. <c>public Guid CurrentTenantId => _tenantProvider.TenantId;</c>
/// </summary>
public interface ITenantDbContext<TTenantId>
{
    TTenantId CurrentTenantId { get; }
}
