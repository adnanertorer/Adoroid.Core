using Adoroid.Core.Repository.Paging.ExceptionMessages;
using Microsoft.EntityFrameworkCore;

namespace Adoroid.Core.Repository.Paging;

public static class IQuaryablePaginateExtensions
{
    public static async Task<Paginate<T>> ToPaginateAsync<T>(this IQueryable<T> query, int index, int size, 
        CancellationToken cancellationToken = default, bool isAllItems = false)
    {
        if (isAllItems is true)
        {
            var count = await query.CountAsync(cancellationToken).ConfigureAwait(false);
            size = count;
            index = 0;
            var items = count > 0
                ? await query.ToListAsync(cancellationToken).ConfigureAwait(false)
                : [];

            return new Paginate<T>
            {
                Count = count,
                Index = index,
                Items = items,
                Pages = (int)Math.Ceiling(count / (double)size),
                Size = size
            };
        }
        else
        {
            if (index < 0) throw new ArgumentException(Messages.IndexMustBeGreaterThanOrEqualZero, nameof(index));
            if (size <= 0) throw new ArgumentException(Messages.SizeMustBeGreaterThanZero, nameof(size));

            var count = await query.CountAsync(cancellationToken).ConfigureAwait(false);

            var items = count > 0
                ? await query.Skip(index * size).Take(size).ToListAsync(cancellationToken).ConfigureAwait(false)
                : [];

            return new Paginate<T>
            {
                Count = count,
                Index = index,
                Items = items,
                Pages = (int)Math.Ceiling(count / (double)size),
                Size = size
            };
        }
        
    }

    public static Paginate<T> ToPaginate<T>(
       this IQueryable<T> source,
       int index, int size)
    {
        if (index < 0) throw new ArgumentException(Messages.IndexMustBeGreaterThanOrEqualZero, nameof(index));
        if (size <= 0) throw new ArgumentException(Messages.SizeMustBeGreaterThanZero, nameof(size));

        var count = source.Count();
        var items = count > 0 ? source.Skip(index * size).Take(size).ToList() : [];

        Paginate<T> list = new()
        {
            Count = count,
            Index = index,
            Items = items,
            Pages = (int)Math.Ceiling(count / (double)size),
            Size = size
        };
        return list;
    }
}
