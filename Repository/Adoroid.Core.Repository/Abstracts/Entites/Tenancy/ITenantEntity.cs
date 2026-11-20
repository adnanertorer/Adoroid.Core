namespace Adoroid.Core.Repository.Abstracts.Entites.Tenancy;

public interface ITenantEntity<TTenantId>
{
    TTenantId TenantId { get; }
}

/// <summary>
/// TenantId'nin yalnızca kütüphane içinden (repository) atanabilmesi için.
/// </summary>
internal interface ITenantAssignable<TTenantId>
{
    void AssignTenant(TTenantId tenantId);
}
