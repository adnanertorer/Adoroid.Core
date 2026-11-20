namespace Adoroid.Core.Repository.Contracts;

public interface ICurrentUserProvider
{
    Guid? UserId { get; }
}
