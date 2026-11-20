using Adoroid.Core.Repository.Abstracts.Entites.Tenancy;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Adoroid.Core.Repository.Tenancy;

public static class TenantModelBuilderExtensions
{
    /// <summary>
    /// <see cref="ITenantEntity{TTenantId}"/> uygulayan tüm kök entity tiplerine tenant global query filter'ı ekler.
    /// Böylece Include ile yüklenen ilişkili tenant entity'leri de filtrelenir.
    /// <para>
    /// OnModelCreating içinde, entity konfigürasyonlarından ve diğer HasQueryFilter çağrılarından SONRA çağırın;
    /// mevcut filtreler AND ile birleştirilir. <paramref name="context"/> olarak DbContext'in kendisini (<c>this</c>) verin.
    /// </para>
    /// </summary>
    /// <example>
    /// protected override void OnModelCreating(ModelBuilder modelBuilder)
    /// {
    ///     base.OnModelCreating(modelBuilder);
    ///     modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    ///     modelBuilder.ApplyTenantQueryFilters(this);
    /// }
    /// </example>
    public static ModelBuilder ApplyTenantQueryFilters<TTenantId>(this ModelBuilder modelBuilder, ITenantDbContext<TTenantId> context)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);
        if (context is not DbContext)
            throw new ArgumentException("context must be the DbContext instance itself (pass 'this').", nameof(context));

        // DbContext örneğine yapılan erişimi EF her sorguda o anki context'e göre parametreler.
        var currentTenantId = Expression.Property(
            Expression.Constant(context, context.GetType()),
            typeof(ITenantDbContext<TTenantId>).GetProperty(nameof(ITenantDbContext<TTenantId>.CurrentTenantId))!);

        foreach (var entityType in modelBuilder.Model.GetEntityTypes().ToList())
        {
            var clrType = entityType.ClrType;
            if (entityType.BaseType is not null
                || entityType.IsOwned()
                || !typeof(ITenantEntity<TTenantId>).IsAssignableFrom(clrType))
                continue;

            var e = Expression.Parameter(clrType, "e");
            Expression body = Expression.Equal(
                Expression.Property(e, nameof(ITenantEntity<TTenantId>.TenantId)),
                currentTenantId);

            var existingFilter = entityType.GetQueryFilter();
            if (existingFilter is not null)
                body = Expression.AndAlso(
                    ParameterReplacer.Replace(existingFilter.Body, existingFilter.Parameters[0], e),
                    body);

            modelBuilder.Entity(clrType).HasQueryFilter(Expression.Lambda(body, e));
        }

        return modelBuilder;
    }
}
