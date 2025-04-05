namespace Adoroid.Core.Application.Requests;

public class PageRequest
{
    public int PageIndex { get; set; }
    public int PageSize { get; set; }
    public bool IsAllItems {get; set;} = false;
}
