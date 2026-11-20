using Adoroid.Core.Repository.Abstracts.Entites.Tenancy;
using Adoroid.Core.Repository.Contracts;
using Adoroid.Core.Repository.Contracts.Tenancy;
using Adoroid.Core.Repository.Dynamics;
using Adoroid.Core.Repository.Paging;
using Adoroid.Core.Repository.Tenancy;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Adoroid.Core.Repository.Repositories;

/// <summary>
/// Tenant'a duyarlı repository.
/// <list type="bullet">
/// <item>Tüm sorgular aktif tenant ile filtrelenir; tenant çözülemezse <see cref="TenantNotResolvedException"/> fırlatılır.</item>
/// <item>Add: TenantId otomatik atanır; entity'de başka bir tenant varsa <see cref="TenantAccessException"/> fırlatılır.</item>
/// <item>Update/Delete: takip edilmeyen entity'lerin aktif tenant'a ait olduğu veritabanında doğrulanır; TenantId değiştirilemez.</item>
/// <item>Yazma metotları SaveChanges çağırmaz.</item>
/// </list>
/// </summary>
public class TenantRepositoryBase<TEntity, TEntityId, TContext, TTenantId>
    : IAsyncTenantRepository<TEntity, TEntityId, TTenantId>, ITenantRepository<TEntity, TEntityId, TTenantId>
    where TEntity : TenantEntity<TEntityId, TTenantId>
    where TContext : DbContext
{
    protected readonly TContext Context;
    protected readonly ICurrentUserProvider CurrentUserProvider;
    protected readonly ICurrentTenantProvider<TTenantId> CurrentTenantProvider;

    public TenantRepositoryBase(TContext context, ICurrentUserProvider currentUserProvider, ICurrentTenantProvider<TTenantId> currentTenantProvider)
    {
        Context = context ?? throw new ArgumentNullException(nameof(context));
        CurrentUserProvider = currentUserProvider ?? throw new ArgumentNullException(nameof(currentUserProvider));
        CurrentTenantProvider = currentTenantProvider ?? throw new ArgumentNullException(nameof(currentTenantProvider));
    }

    #region Query

    public IQueryable<TEntity> Query()
        => Context.Set<TEntity>().Where(TenantExpressions.TenantEquals<TEntity, TTenantId>(RequireTenant()));

    protected IQueryable<TEntity> BuildQuery(
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include,
        bool withDeleted,
        bool enableTracking)
    {
        var queryable = Query();

        if (!enableTracking)
            queryable = queryable.AsNoTracking();

        if (withDeleted)
            queryable = queryable.IgnoreQueryFilters(); // Query() içindeki tenant koşulu yerinde kalır.
        else
            queryable = queryable.Where(e => !e.IsDeleted);

        if (include != null)
            queryable = include(queryable);

        return queryable;
    }

    private static IQueryable<TEntity> ApplyPredicate(IQueryable<TEntity> queryable, Expression<Func<TEntity, bool>>? predicate)
        => predicate is null ? queryable : queryable.Where(predicate);

    public async Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>> predicate, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        bool withDeleted = false, bool enableTracking = true, CancellationToken cancellationToken = default)
    {
        return await BuildQuery(include, withDeleted, enableTracking).FirstOrDefaultAsync(predicate, cancellationToken);
    }

    public async Task<Paginate<TEntity>> GetListAsync(Expression<Func<TEntity, bool>>? predicate = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null, int index = 0, int size = 10, bool withDeleted = false, bool enableTracking = true,
        CancellationToken cancellationToken = default)
    {
        var queryable = ApplyPredicate(BuildQuery(include, withDeleted, enableTracking), predicate);
        if (orderBy != null)
            return await orderBy(queryable).ToPaginateAsync(index, size, cancellationToken);
        return await queryable.ToPaginateAsync(index, size, cancellationToken);
    }

    public async Task<Paginate<TEntity>> GetListByDynamicAsync(DynamicQuery dynamicQuery, Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        int index = 0, int size = 10, bool withDeleted = false, bool enableTracking = true, CancellationToken cancellationToken = default)
    {
        var queryable = ApplyPredicate(BuildQuery(include, withDeleted, enableTracking).ToDynamic(dynamicQuery), predicate);
        if (orderBy != null)
            return await orderBy(queryable).ToPaginateAsync(index, size, cancellationToken);
        return await queryable.ToPaginateAsync(index, size, cancellationToken);
    }

    public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>>? predicate = null, bool withDeleted = false, bool enableTracking = true,
        CancellationToken cancellationToken = default)
    {
        return await ApplyPredicate(BuildQuery(null, withDeleted, enableTracking), predicate).AnyAsync(cancellationToken);
    }

    public TEntity? Get(Expression<Func<TEntity, bool>> predicate, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        bool withDeleted = false, bool enableTracking = true, CancellationToken cancellationToken = default)
    {
        return BuildQuery(include, withDeleted, enableTracking).FirstOrDefault(predicate);
    }

    public Paginate<TEntity> GetList(Expression<Func<TEntity, bool>>? predicate = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null, int index = 0, int size = 10, bool withDeleted = false, bool enableTracking = true,
        CancellationToken cancellationToken = default)
    {
        var queryable = ApplyPredicate(BuildQuery(include, withDeleted, enableTracking), predicate);
        if (orderBy != null)
            return orderBy(queryable).ToPaginate(index, size);
        return queryable.ToPaginate(index, size);
    }

    public Paginate<TEntity> GetListByDynamic(DynamicQuery dynamicQuery, Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        int index = 0, int size = 10, bool withDeleted = false, bool enableTracking = true, CancellationToken cancellationToken = default)
    {
        var queryable = ApplyPredicate(BuildQuery(include, withDeleted, enableTracking).ToDynamic(dynamicQuery), predicate);
        if (orderBy != null)
            return orderBy(queryable).ToPaginate(index, size);
        return queryable.ToPaginate(index, size);
    }

    public bool Any(Expression<Func<TEntity, bool>>? predicate = null, bool withDeleted = false, bool enableTracking = true,
        CancellationToken cancellationToken = default)
    {
        return ApplyPredicate(BuildQuery(null, withDeleted, enableTracking), predicate).Any();
    }

    #endregion

    #region Commands (async)

    public async Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        PrepareForAdd(entity, RequireTenant());
        await Context.AddAsync(entity, cancellationToken);
        return entity;
    }

    public async Task<ICollection<TEntity>> AddRangeAsync(ICollection<TEntity> entities, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entities);
        var tenantId = RequireTenant();
        foreach (var entity in entities)
            PrepareForAdd(entity, tenantId);

        await Context.AddRangeAsync(entities, cancellationToken);
        return entities;
    }

    public async Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        await EnsureOwnedAsync(new[] { entity }, RequireTenant(), cancellationToken);
        MarkUpdated(entity);
        return entity;
    }

    public async Task<ICollection<TEntity>> UpdateRangeAsync(ICollection<TEntity> entities, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entities);
        await EnsureOwnedAsync(entities, RequireTenant(), cancellationToken);
        foreach (var entity in entities)
            MarkUpdated(entity);
        return entities;
    }

    public async Task<TEntity> DeleteAsync(TEntity entity, bool permanent = false, CancellationToken cancellationToken = default)
    {
        await EnsureOwnedAsync(new[] { entity }, RequireTenant(), cancellationToken);
        MarkDeleted(entity, permanent);
        return entity;
    }

    public async Task<ICollection<TEntity>> DeleteRangeAsync(ICollection<TEntity> entities, bool permanent = false, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entities);
        await EnsureOwnedAsync(entities, RequireTenant(), cancellationToken);
        foreach (var entity in entities)
            MarkDeleted(entity, permanent);
        return entities;
    }

    #endregion

    #region Commands (sync)

    public TEntity Add(TEntity entity)
    {
        PrepareForAdd(entity, RequireTenant());
        Context.Add(entity);
        return entity;
    }

    public ICollection<TEntity> AddRange(ICollection<TEntity> entities)
    {
        ArgumentNullException.ThrowIfNull(entities);
        var tenantId = RequireTenant();
        foreach (var entity in entities)
            PrepareForAdd(entity, tenantId);

        Context.AddRange(entities);
        return entities;
    }

    public TEntity Update(TEntity entity)
    {
        EnsureOwned(new[] { entity }, RequireTenant());
        MarkUpdated(entity);
        return entity;
    }

    public ICollection<TEntity> UpdateRange(ICollection<TEntity> entities)
    {
        ArgumentNullException.ThrowIfNull(entities);
        EnsureOwned(entities, RequireTenant());
        foreach (var entity in entities)
            MarkUpdated(entity);
        return entities;
    }

    public TEntity Delete(TEntity entity, bool permanent = false)
    {
        EnsureOwned(new[] { entity }, RequireTenant());
        MarkDeleted(entity, permanent);
        return entity;
    }

    public ICollection<TEntity> DeleteRange(ICollection<TEntity> entities, bool permanent = false)
    {
        ArgumentNullException.ThrowIfNull(entities);
        EnsureOwned(entities, RequireTenant());
        foreach (var entity in entities)
            MarkDeleted(entity, permanent);
        return entities;
    }

    #endregion

    #region Tenant helpers
    protected TTenantId RequireTenant()
    {
        var tenantId = CurrentTenantProvider.TenantId;
        if (TenantGuard.IsEmpty(tenantId))
            throw new TenantNotResolvedException();
        return tenantId;
    }

    protected virtual void PrepareForAdd(TEntity entity, TTenantId tenantId)
    {
        ArgumentNullException.ThrowIfNull(entity);
        ((ITenantAssignable<TTenantId>)entity).AssignTenant(tenantId);
        entity.SetAuditCreated(DateTimeOffset.UtcNow, CurrentUserProvider.UserId);
    }

    protected async Task EnsureOwnedAsync(IEnumerable<TEntity> entities, TTenantId tenantId, CancellationToken cancellationToken)
    {
        var ids = CollectIdsToVerify(entities, tenantId);
        if (ids.Count == 0)
            return;

        var owned = await Context.Set<TEntity>().IgnoreQueryFilters().AsNoTracking()
            .CountAsync(TenantExpressions.IdsAndTenantEquals<TEntity, TEntityId, TTenantId>(ids, tenantId), cancellationToken);

        if (owned != ids.Count)
            throw new TenantAccessException();
    }

    protected void EnsureOwned(IEnumerable<TEntity> entities, TTenantId tenantId)
    {
        var ids = CollectIdsToVerify(entities, tenantId);
        if (ids.Count == 0)
            return;

        var owned = Context.Set<TEntity>().IgnoreQueryFilters().AsNoTracking()
            .Count(TenantExpressions.IdsAndTenantEquals<TEntity, TEntityId, TTenantId>(ids, tenantId));

        if (owned != ids.Count)
            throw new TenantAccessException();
    }

    private List<TEntityId> CollectIdsToVerify(IEnumerable<TEntity> entities, TTenantId tenantId)
    {
        var ids = new HashSet<TEntityId>();
        foreach (var entity in entities)
        {
            ArgumentNullException.ThrowIfNull(entity);

            if (!TenantGuard.AreEqual(entity.TenantId, tenantId))
                throw new TenantAccessException();

            var entry = Context.Entry(entity);
            if (entry.State == EntityState.Detached)
            {
                ids.Add(entity.Id);
                continue;
            }

            if (!TenantGuard.AreEqual(entry.Property(e => e.TenantId).OriginalValue, tenantId))
                throw new TenantAccessException();
        }

        return ids.ToList();
    }

    private void MarkUpdated(TEntity entity)
    {
        entity.SetAuditUpdated(DateTimeOffset.UtcNow, CurrentUserProvider.UserId);
        AttachAsModified(entity);
    }

    private void MarkDeleted(TEntity entity, bool permanent)
    {
        if (permanent)
        {
            Context.Remove(entity);
            return;
        }

        CheckHasEntityOneToOneRelation(entity);
        entity.SoftDelete(CurrentUserProvider.UserId);
        AttachAsModified(entity);
    }

    /// <summary>
    /// Takip edilmeyen entity'yi Modified olarak bağlar (takip edilenleri change tracker zaten izler)
    /// ve TenantId kolonunu UPDATE dışında bırakır.
    /// </summary>
    private void AttachAsModified(TEntity entity)
    {
        if (Context.Entry(entity).State == EntityState.Detached)
            Context.Update(entity);

        var entry = Context.Entry(entity);
        if (entry.State == EntityState.Modified)
            entry.Property(e => e.TenantId).IsModified = false;
    }

    protected void CheckHasEntityOneToOneRelation(TEntity entity)
    {
        var hasEntityHaveOneToOneRelation = Context.Entry(entity)
            .Metadata.GetForeignKeys().All(
                x => x.DependentToPrincipal?.IsCollection == true
                     || x.PrincipalToDependent?.IsCollection == true
                     || x.DependentToPrincipal?.ForeignKey.DeclaringEntityType.ClrType == entity.GetType()
            ) == false;
        if (hasEntityHaveOneToOneRelation)
            throw new InvalidOperationException("Entity has one-to-one relationship.");
    }

    #endregion
}
