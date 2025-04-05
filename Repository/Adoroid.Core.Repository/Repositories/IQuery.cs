namespace Adoroid.Core.Repository.Repositories;

public interface IQuery<T>
{
    IQueryable<T> Query();
}

