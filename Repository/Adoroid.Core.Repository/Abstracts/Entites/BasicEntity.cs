namespace Adoroid.Core.Repository.Abstracts.Entites;

public abstract class BasicEntity<TId> 
{
    public required TId Id { get; set; }

    public BasicEntity()
    {
        Id = default!;
    }

    public BasicEntity(TId id)
    {
        if (id is null)
            throw new ArgumentNullException(nameof(id), "Id cannot be null.");

        Id = id;
    }
}
