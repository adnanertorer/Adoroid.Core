namespace Adoroid.Core.Repository.Repositories;

public class Entity<TId> : IEntityTimestamps
{
    public TId Id { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public DateTime? DeletedDate { get; set; }
    public TId CreatedBy { get; set; }
    public TId? UpdatedBy { get; set; }
    public TId? DeletedBy { get; set; }
    public bool? IsDeleted { get; set; }

    public Entity()
    {
        Id = default;
    }

    public Entity(TId id)
    {
        Id = id;
    }
}
