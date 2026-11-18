using Adoroid.Core.Repository.Abstracts.Entites;
using Adoroid.Core.Repository.Abstracts.Entites.Tenancy;
using Adoroid.Core.Repository.Contracts;
using Adoroid.Core.Repository.Contracts.Tenancy;
using Adoroid.Core.Repository.Dynamics;
using Adoroid.Core.Repository.Paging;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;

namespace Adoroid.Core.Repository.Repositories;

public class TenantRepositoryBase<TEntity, TEntityId, TContext, TTenantId> : IAsyncTenantRepository<TEntity, TEntityId>, ITenantRepository<TEntity, TEntityId>
 where TEntity : TenantEntity<TEntityId> where TContext : DbContext
{
    protected readonly TContext Context;
    protected readonly ICurrentUserProvider CurrentUserProvider;
    protected readonly ICurrentTenantProvider<TTenantId> CurrentTenantProvider;


    public TenantRepositoryBase(TContext context, ICurrentUserProvider currentUserProvider, ICurrentTenantProvider<TTenantId> currentTenantProvider)
    {
        this.Context = context;
        this.CurrentUserProvider = currentUserProvider;
        this.CurrentTenantProvider = currentTenantProvider;
    }

    public IQueryable<TEntity> Query() => this.Context.Set<TEntity>().Where(i => i.TenantId != null && i.TenantId.Equals(this.CurrentTenantProvider.TenantId));

    public async Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>> predicate, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null, bool withDeleted = false, bool enableTracking = true,
        CancellationToken cancellationToken = default)
    {
        var queryable = Query();
        if (enableTracking)
            queryable = queryable.AsNoTracking();
        if (include != null)
            queryable = include(queryable);
        if (withDeleted)
            queryable = queryable.IgnoreQueryFilters();
        return await queryable.FirstOrDefaultAsync(predicate, cancellationToken);
    }

    public async Task<Paginate<TEntity>> GetListAsync(Expression<Func<TEntity, bool>>? predicate = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null, int index = 0,
        int size = 10, bool withDeleted = false, bool enableTracking = true, CancellationToken cancellationToken = default)
    {
        var queryable = Query();
        if (enableTracking)
            queryable = queryable.AsNoTracking();
        if (include != null)
            queryable = include(queryable);
        if (withDeleted)
            queryable = queryable.IgnoreQueryFilters();
        if (predicate != null)
            queryable = queryable.Where(predicate);
        if (orderBy != null)
            return await orderBy(queryable).ToPaginateAsync(index, size, cancellationToken);
        return await queryable.ToPaginateAsync(index, size, cancellationToken);
    }

    public async Task<Paginate<TEntity>> GetListByDynamicAsync(DynamicQuery dynamicQuery, Expression<Func<TEntity, bool>>? predicate = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null, int index = 0, int size = 10, bool withDeleted = false, bool enableTracking = true,
        CancellationToken cancellationToken = default)
    {
        var queryable = Query().ToDynamic(dynamicQuery);
        if (enableTracking)
            queryable = queryable.AsNoTracking();
        if (include != null)
            queryable = include(queryable);
        if (withDeleted)
            queryable = queryable.IgnoreQueryFilters();
        if (predicate != null)
            queryable = queryable.Where(predicate);
        if (orderBy != null)
            return await orderBy(queryable).ToPaginateAsync(index, size, cancellationToken);
        return await queryable.ToPaginateAsync(index, size, cancellationToken);
    }

    public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>>? predicate = null, bool withDeleted = false, bool enableTracking = true,
        CancellationToken cancellationToken = default)
    {
        var queryable = Query();
        if (enableTracking)
            queryable = queryable.AsNoTracking();
        if (withDeleted)
            queryable = queryable.IgnoreQueryFilters();
        if (predicate != null)
            queryable = queryable.Where(predicate);
        return await queryable.AnyAsync(cancellationToken);
    }

    public async Task<TEntity> AddAsync(TEntity entity)
    {
        entity.SetAuditCreated(DateTimeOffset.UtcNow, entity.CreatedBy);
        await Context.AddAsync(entity);
        await Context.SaveChangesAsync();
        return entity;
    }

    public async Task<ICollection<TEntity>> AddRangeAsync(ICollection<TEntity> entities)
    {
        foreach (var tEntity in entities)
            tEntity.SetAuditCreated(DateTimeOffset.UtcNow, tEntity.CreatedBy);

        await Context.AddRangeAsync(entities);
        await Context.SaveChangesAsync();
        return entities;
    }

    public async Task<TEntity> UpdateAsync(TEntity entity)
    {
        entity.SetAuditUpdated(DateTimeOffset.UtcNow, entity.CreatedBy);
        Context.Update(entity);
        await Context.SaveChangesAsync();
        return entity;
    }

    public async Task<ICollection<TEntity>> UpdateRangeAsync(ICollection<TEntity> entities)
    {
        foreach (var tEntity in entities)
            tEntity.SetAuditUpdated(DateTimeOffset.UtcNow, tEntity.CreatedBy);

        Context.UpdateRange(entities);
        await Context.SaveChangesAsync();
        return entities;
    }

    public async Task<TEntity> DeleteAsync(TEntity entity, bool permanent = false)
    {
        if(!permanent)
            
        await SetEntityDeletedAsync(entity, permanent);
        await Context.SaveChangesAsync();
        return entity;
    }

    public async Task<ICollection<TEntity>> DeleteRangeAsync(ICollection<TEntity> entities, bool permanent = false)
    {
        await SetEntityAsDeletedAsync(entities, permanent);
        await Context.SaveChangesAsync();
        return entities;
    }

    public TEntity? Get(Expression<Func<TEntity, bool>> predicate, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null, bool withDeleted = false, bool enableTracking = true,
        CancellationToken cancellationToken = default)
    {
        var queryable = Query();
        if (enableTracking)
            queryable = queryable.AsNoTracking();
        if (include != null)
            queryable = include(queryable);
        if (withDeleted)
            queryable = queryable.IgnoreQueryFilters();
        return queryable.FirstOrDefault(predicate);
    }

    public Paginate<TEntity> GetList(Expression<Func<TEntity, bool>>? predicate = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null, int index = 0, int size = 10,
        bool withDeleted = false, bool enableTracking = true, CancellationToken cancellationToken = default)
    {
        var queryable = Query();
        if (enableTracking)
            queryable = queryable.AsNoTracking();
        if (include != null)
            queryable = include(queryable);
        if (withDeleted)
            queryable = queryable.IgnoreQueryFilters();
        if (predicate != null)
            queryable = queryable.Where(predicate);
        if (orderBy != null)
            return orderBy(queryable).ToPaginate(index, size);
        return queryable.ToPaginate(index, size);
    }

    public Paginate<TEntity> GetListByDynamic(DynamicQuery dynamicQuery, Expression<Func<TEntity, bool>>? predicate = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null, int index = 0, int size = 10, bool withDeleted = false, bool enableTracking = true,
        CancellationToken cancellationToken = default)
    {
        var queryable = Query().ToDynamic(dynamicQuery);
        if (enableTracking)
            queryable = queryable.AsNoTracking();
        if (include != null)
            queryable = include(queryable);
        if (withDeleted)
            queryable = queryable.IgnoreQueryFilters();
        if (predicate != null)
            queryable = queryable.Where(predicate);
        if (orderBy != null)
            return orderBy(queryable).ToPaginate(index, size);
        return queryable.ToPaginate(index, size);
    }

    public bool Any(Expression<Func<TEntity, bool>>? predicate = null, bool withDeleted = false, bool enableTracking = true,
        CancellationToken cancellationToken = default)
    {
        var queryable = Query();
        if (enableTracking)
            queryable = queryable.AsNoTracking();
        if (withDeleted)
            queryable = queryable.IgnoreQueryFilters();
        if (predicate != null)
            queryable = queryable.Where(predicate);
        return queryable.Any();
    }

    public TEntity Add(TEntity entity)
    {
        entity.SetAuditCreated(DateTimeOffset.UtcNow, CurrentUserProvider.UserId);
        Context.Add(entity);
        Context.SaveChanges();
        return entity;
    }

    public ICollection<TEntity> AddRange(ICollection<TEntity> entities)
    {
        foreach (var tEntity in entities)
            tEntity.SetAuditCreated(DateTimeOffset.UtcNow, CurrentUserProvider.UserId);

        Context.AddRange(entities);
        Context.SaveChanges();
        return entities;
    }

    public TEntity Update(TEntity entity)
    {
        entity.SetAuditUpdated(DateTimeOffset.UtcNow, CurrentUserProvider.UserId);
        Context.Update(entity);
        Context.SaveChanges();
        return entity;
    }

    public ICollection<TEntity> UpdateRange(ICollection<TEntity> entities)
    {
        foreach (var tEntity in entities)
            tEntity.SetAuditUpdated(DateTimeOffset.UtcNow, CurrentUserProvider.UserId);

        Context.UpdateRange(entities);
        Context.SaveChanges();
        return entities;
    }

    public TEntity Delete(TEntity entity, bool permanent = false)
    {
        if (permanent)
        {
            Context.Remove(entity);
        }
        else
        {
            entity.SoftDelete(CurrentUserProvider.UserId);
            Context.Update(entity);
        }
        Context.SaveChanges();
        return entity;
    }

    public ICollection<TEntity> DeleteRange(ICollection<TEntity> entities, bool permanent = false)
    {
        if (permanent)
        {
            foreach (var tEntity in entities)
            {
                Context.Remove(tEntity);
            }
        }
        else
        {
            foreach (var tEntity in entities)
            {
                tEntity.SoftDelete(CurrentUserProvider.UserId);
            }
            Context.UpdateRange(entities);
        }
        Context.SaveChanges();
        return entities;
    }

    protected async Task SetEntityDeletedAsync(TEntity entity, bool permanent)
    {
        if (!permanent)
        {
            CheckHasEntityOneToOneRelation(entity);
            entity.SoftDelete(CurrentUserProvider.UserId);
        }
        else
        {
            Context.Remove(entity);
        }
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

    

    private IQueryable<object> GetRelationLoaderQuery(IQueryable query, Type navigationPropertyType)
    {
        var queryProviderType = query.Provider.GetType();
        var createQueryMethod =
            queryProviderType
                .GetMethods()
                .First(m => m is { Name: nameof(query.Provider.CreateQuery), IsGenericMethod: true })
                ?.MakeGenericMethod(navigationPropertyType)
            ?? throw new InvalidOperationException("CreateQuery<TElement> method is not found in IQueryProvider.");
        var queryProviderQuery =
            (IQueryable<object>)createQueryMethod.Invoke(query.Provider, parameters: [query.Expression])!;
        return queryProviderQuery.Where(x => !((IEntityTimestamps)x).DeletedDate.HasValue);
    }

    protected async Task SetEntityAsDeletedAsync(IEnumerable<TEntity> entities, bool permanent)
    {
        var enumerable = entities as TEntity[] ?? entities.ToArray();
        foreach (var entity in enumerable)
        {
            entity.SoftDelete(CurrentUserProvider.UserId);
        }

        await SetEntityAsDeletedAsync(enumerable, permanent);
    }
}
