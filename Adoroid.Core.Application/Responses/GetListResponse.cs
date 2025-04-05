using IdealSigorta.Core.Repository.Paging;

namespace Adoroid.Core.Application.Responses;

public class GetListResponse<T> : BasePageableModule
{
    private IList<T>? _items;
    public IList<T> Items
    {
        get => _items ??= [];
        set => _items = value;
    }
}
