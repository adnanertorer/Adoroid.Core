﻿using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;
using Adoroid.Core.Repository.Dynamics;
using Adoroid.Core.Repository.Paging;

namespace Adoroid.Core.Repository.Repositories;

public interface IRepository<TEntity, TEntityId> : IQuery<TEntity> where TEntity : Entity<TEntityId>
{
    TEntity? Get(
        Expression<Func<TEntity, bool>> predicate,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        bool withDeleted = false,
        bool enableTracking = true, CancellationToken cancellationToken = default);

    Paginate<TEntity> GetList(
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        int index = 0,
        int size = 10,
        bool withDeleted = false,
        bool enableTracking = true, CancellationToken cancellationToken = default);

    Paginate<TEntity> GetListByDynamic(
        DynamicQuery dynamicQuery,
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        int index = 0,
        int size = 10,
        bool withDeleted = false,
        bool enableTracking = true, CancellationToken cancellationToken = default);

    bool Any(
        Expression<Func<TEntity, bool>>? predicate = null,
        bool withDeleted = false,
        bool enableTracking = true, CancellationToken cancellationToken = default);

    TEntity Add(TEntity entity);
    ICollection<TEntity> AddRange(ICollection<TEntity> entities);
    TEntity Update(TEntity entity);
    ICollection<TEntity> UpdateRange(ICollection<TEntity> entities);
    TEntity Delete(TEntity entity, bool permanent = false);
    ICollection<TEntity> DeleteRange(ICollection<TEntity> entities, bool permanent = false);
}
