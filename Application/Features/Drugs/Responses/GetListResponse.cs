using Persistence.Paging;

namespace Application.Features.Drugs.Responses;

public class GetListResponse<T> : BasePageableModel
{
    private IList<T> _items;

    public IList<T> Items 
    { 
        get => _items; 
        set => _items = value; 
    }
}
