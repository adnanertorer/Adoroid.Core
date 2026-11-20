using System.Linq.Expressions;

namespace Adoroid.Core.Repository.Tenancy;

/// <summary>
/// Generic tipler üzerinde EF Core'un SQL'e çevirebileceği, parametreli (sabit gömülmeyen) ifadeler üretir.
/// </summary>
internal static class TenantExpressions
{
    private const string TenantIdProperty = "TenantId";
    private const string IdProperty = "Id";

    public static Expression<Func<TEntity, bool>> TenantEquals<TEntity, TTenantId>(TTenantId tenantId)
    {
        var e = Expression.Parameter(typeof(TEntity), "e");
        var body = Expression.Equal(Expression.Property(e, TenantIdProperty), Parameter(tenantId));
        return Expression.Lambda<Func<TEntity, bool>>(body, e);
    }

    public static Expression<Func<TEntity, bool>> IdsAndTenantEquals<TEntity, TId, TTenantId>(IReadOnlyCollection<TId> ids, TTenantId tenantId)
    {
        var e = Expression.Parameter(typeof(TEntity), "e");

        var idIn = Expression.Call(
            typeof(Enumerable),
            nameof(Enumerable.Contains),
            new[] { typeof(TId) },
            Parameter<IEnumerable<TId>>(ids),
            Expression.Property(e, IdProperty));

        var tenantEquals = Expression.Equal(Expression.Property(e, TenantIdProperty), Parameter(tenantId));

        return Expression.Lambda<Func<TEntity, bool>>(Expression.AndAlso(idIn, tenantEquals), e);
    }

    // Değeri bir nesnenin property'si üzerinden vermek, EF'in onu SQL parametresi yapmasını sağlar.
    private static MemberExpression Parameter<T>(T value)
        => Expression.Property(Expression.Constant(new ValueHolder<T>(value)), nameof(ValueHolder<T>.Value));

    private sealed class ValueHolder<T>(T value)
    {
        public T Value { get; } = value;
    }
}

internal sealed class ParameterReplacer(ParameterExpression from, Expression to) : ExpressionVisitor
{
    public static Expression Replace(Expression body, ParameterExpression from, Expression to)
        => new ParameterReplacer(from, to).Visit(body);

    protected override Expression VisitParameter(ParameterExpression node)
        => node == from ? to : base.VisitParameter(node);
}
