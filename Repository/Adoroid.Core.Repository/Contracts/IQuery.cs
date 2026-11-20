namespace Adoroid.Core.Repository.Contracts;

public interface IQuery<T>
{
    IQueryable<T> Query();
}

