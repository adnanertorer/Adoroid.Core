namespace Adoroid.Core.Repository.Repositories;

public interface IEntityTimestamps
{
    DateTime? CreatedDate { get; set; }
    DateTime? UpdatedDate { get; set; }
    DateTime? DeletedDate { get; set; }
    public bool? IsDeleted { get; set; }
}
