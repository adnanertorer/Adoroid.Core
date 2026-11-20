namespace Adoroid.Core.Repository.Contracts.Tenancy;

public interface ICurrentTenantProvider<TId>
{
    TId TenantId { get; }
}
