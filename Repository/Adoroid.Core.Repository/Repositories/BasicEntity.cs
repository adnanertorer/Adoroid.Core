namespace Adoroid.Core.Repository.Repositories;

public abstract class BasicEntity<TId> 
{
    public required TId Id { get; set; }

    public BasicEntity()
    {
        if(Id is null)
            throw new ArgumentNullException(nameof(Id), "Id cannot be null.");

        Id = default!;
    }

    public BasicEntity(TId id)
    {
        if (id is null)
            throw new ArgumentNullException(nameof(id), "Id cannot be null.");

        Id = id;
    }
}
