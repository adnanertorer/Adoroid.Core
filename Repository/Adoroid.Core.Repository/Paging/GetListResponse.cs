namespace Adoroid.Core.Repository.Paging;

public class GetListResponse<T> : BasePageableModule
{
    private IList<T>? _items;
    public IList<T> Items
    {
        get => _items ??= [];
        set => _items = value;
    }
}