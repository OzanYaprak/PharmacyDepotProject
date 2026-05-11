using Persistence.Paging;

namespace Application.Common.Responses;

/// <summary>
/// Sayfalanmış liste sorguları için kullanılan ortak yanıt modeli.
/// </summary>
/// <typeparam name="T">Liste elemanının tipi.</typeparam>
public class GetListResponse<T> : BasePageableModel
{
    private IList<T> _items;

    public IList<T> Items
    {
        get => _items ??= new List<T>();
        set => _items = value;
    }
}
