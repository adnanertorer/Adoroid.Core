namespace Adoroid.Core.Repository.Tenancy;

/// <summary>
/// Aktif tenant çözülemediğinde (ör. TenantId boş/default geldiğinde) fırlatılır.
/// </summary>
public class TenantNotResolvedException : InvalidOperationException
{
    public TenantNotResolvedException() : base("Current tenant could not be resolved.") { }
    public TenantNotResolvedException(string message) : base(message) { }
}

/// <summary>
/// Bir entity aktif tenant'a ait olmadığında fırlatılır.
/// </summary>
public class TenantAccessException : InvalidOperationException
{
    public TenantAccessException() : base("Entity does not belong to the current tenant.") { }
    public TenantAccessException(string message) : base(message) { }
}
