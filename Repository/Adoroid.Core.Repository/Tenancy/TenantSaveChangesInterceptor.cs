using Adoroid.Core.Repository.Abstracts.Entites.Tenancy;
using Adoroid.Core.Repository.Contracts.Tenancy;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Adoroid.Core.Repository.Tenancy;

/// <summary>
/// SaveChanges sırasında tenant kurallarını uygular:
/// Added entity'lere aktif TenantId'yi atar; Modified/Deleted entity'lerin orijinal TenantId'sinin
/// aktif tenant olduğunu ve TenantId'nin değiştirilmediğini doğrular.
/// Repository dışından (doğrudan DbContext ile) yapılan yazmalar için de koruma sağlar.
/// </summary>
/// <example>
/// services.AddScoped&lt;TenantSaveChangesInterceptor&lt;Guid&gt;&gt;();
/// services.AddDbContext&lt;AppDbContext&gt;((sp, options) =&gt;
///     options.UseSqlServer(cs).AddInterceptors(sp.GetRequiredService&lt;TenantSaveChangesInterceptor&lt;Guid&gt;&gt;()));
/// </example>
public sealed class TenantSaveChangesInterceptor<TTenantId> : SaveChangesInterceptor
{
    private readonly ICurrentTenantProvider<TTenantId> _currentTenantProvider;

    public TenantSaveChangesInterceptor(ICurrentTenantProvider<TTenantId> currentTenantProvider)
    {
        _currentTenantProvider = currentTenantProvider ?? throw new ArgumentNullException(nameof(currentTenantProvider));
    }

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        ApplyTenantRules(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        ApplyTenantRules(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void ApplyTenantRules(DbContext? context)
    {
        if (context is null)
            return;

        var entries = context.ChangeTracker.Entries<ITenantEntity<TTenantId>>()
            .Where(e => e.State is EntityState.Added or EntityState.Modified or EntityState.Deleted)
            .ToList();

        if (entries.Count == 0)
            return;

        var tenantId = _currentTenantProvider.TenantId;
        if (TenantGuard.IsEmpty(tenantId))
            throw new TenantNotResolvedException();

        foreach (var entry in entries)
        {
            var tenantProperty = entry.Property(nameof(ITenantEntity<TTenantId>.TenantId));

            if (entry.State == EntityState.Added)
            {
                var current = As(tenantProperty.CurrentValue);
                if (TenantGuard.IsEmpty(current))
                    tenantProperty.CurrentValue = tenantId;
                else if (!TenantGuard.AreEqual(current, tenantId))
                    throw new TenantAccessException("Cannot add an entity for another tenant.");

                continue;
            }

            var original = As(tenantProperty.OriginalValue);
            if (!TenantGuard.AreEqual(original, tenantId))
                throw new TenantAccessException();

            if (entry.State == EntityState.Modified && !TenantGuard.AreEqual(As(tenantProperty.CurrentValue), original))
                throw new TenantAccessException("TenantId of an existing entity cannot be changed.");
        }
    }

    private static TTenantId? As(object? value) => value is TTenantId typed ? typed : default;
}
